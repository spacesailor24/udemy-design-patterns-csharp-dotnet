using System;
using System.Collections.Generic;

namespace openClosePrinciple
{
    public enum Color
        {
            Red, Green, Blue
        }

        public enum Size
        {
            Small, Medium, Large
        }
        
        public class Product
        {
            public string Name;
            public Color Color;
            public Size Size;

            public Product(string name, Color color, Size size)
            {
                Name = name ?? throw new ArgumentNullException(paramName: nameof(name));
                Color = color;
                Size = size;
            }
        }
        
        // BAD
            // This is an example of code violating the Open Closed Principle
            // There is a ProductFilter, but the ProductFilter also has added specification
            // AFTER it was already built initially
            public class ProductFilter
            {
                // Initially, filtering by Color was all that was needed
                public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
                {
                    foreach (var p in products)
                        if (p.Color == color)
                            yield return p;
                }
        
                // But then it was requested to be able to filter by Size
                public static IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
                {
                    foreach (var p in products)
                        if (p.Size == size)
                            yield return p;
                }
    
                // Then we needed filtering by Color AND Size
                public static IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> products, Size size, Color color)
                {
                    foreach (var p in products)
                        if (p.Size == size && p.Color == color)
                            yield return p;
                }
                
                // As it's being demostrated above, ProductFilter will continue to grow in size
                // proportional to the amount of specifications needed for the ProductFilter.
                // This violates the Open Close Principle, because it is NOT open for extentsion
                // AND is NOT closed for modification
            }
        //
        
        // GOOD
            // Here is a Specification Interface that a Filter can implement
            public interface ISpecification<T>
            {
                bool IsSatisfied(Product p);
            }
    
            // Here is a Filter Interface that certain Specification can implement
            public interface IFilter<T>
            {
                IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
            }
    
            // Here is the initial specification that the BetterFilter will utilize
            // to filter by Color
            public class ColorSpecification : ISpecification<Product>
            {
                private Color color;
    
                // Expecting a Color to be passed in when instantiated
                public ColorSpecification(Color color)
                {
                    this.color = color;
                }
    
                // Returns whether or not the passed Color matches the color of the Product
                public bool IsSatisfied(Product p)
                {
                    return p.Color == color;
                }
            }
    
            // Here is the next specification that the BetterFilter will utilize
            // to filter by Size
            public class SizeSpecification : ISpecification<Product>
            {
                private Size size;
    
                // Expecting a Size to be passed in when instantiated
                public SizeSpecification(Size size)
                {
                    this.size = size;
                }
    
                // Returns whether or not the passed Color matches the color of the Product
                public bool IsSatisfied(Product p)
                {
                    return p.Size == size;
                }
            }
    
            // In the event that BetterFilter needed to utilize two specifications,
            // a new class could be built to implement this functionality instead of
            // needing to modify the original BetterFilter class
            public class AndSpecification<T> : ISpecification<T>
            {
                private ISpecification<T> first, second;
    
                // Expecting two ISpecifications to be passed in when instantiated
                public AndSpecification(ISpecification<T> first, ISpecification<T> second)
                {
                    this.first = first ?? throw new ArgumentNullException(paramName: nameof(first));
                    this.second = second ?? throw new ArgumentNullException(paramName: nameof(second));
                }
    
                // Return whether both specifications have been satified
                public bool IsSatisfied(Product p)
                {
                    return first.IsSatisfied(p) && second.IsSatisfied(p);
                }
            }
    
            // This is the main class that Products can utilize for filtering using a
            // single ISpecification
            public class BetterFilter : IFilter<Product>
            {
                // Expects a Product and product Specification to be passed in when instantiated
                public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
                {
                    // Checks to see if the ISpecification is satisfied
                    foreach (var i in items)
                        if (spec.IsSatisfied(i))
                            yield return i;
                }
            }
        //
    
    class Program
    {   
        static void Main(string[] args)
        {
            var apple = new Product("Apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var house = new Product("House", Color.Blue, Size.Large);

            Product[] products = {apple, tree, house};

            var pf = new ProductFilter();
            Console.WriteLine("Green products (old):");
            foreach (var p in pf.FilterByColor(products, Color.Green))
                Console.WriteLine($" - {p.Name} is green");

            // ^^ BEFORE

            // vv AFTER
            var bf = new BetterFilter();
            
            Console.WriteLine("Green products (new):");
            foreach (var p in bf.Filter(products, new ColorSpecification(Color.Green)))
                Console.WriteLine($" - {p.Name} is green");

            Console.WriteLine("Large products");
            foreach (var p in bf.Filter(products, new SizeSpecification(Size.Large)))
                Console.WriteLine($" - {p.Name} is large");

            Console.WriteLine("Large blue items");
            foreach (var p in bf.Filter(products,
                new AndSpecification<Product>(new ColorSpecification(Color.Blue), new SizeSpecification(Size.Large)))
            )
            {
                Console.WriteLine($" - {p.Name} is big and blue");
            }
        }
    }
}