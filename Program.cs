using System;

namespace GeometryShapes
{
    // Абстрактний клас для геометричних фігур
    public abstract class Shape
    {
        protected string name;
        
        public abstract string Name { get; set; }
        
        // Абстрактний метод обчислення периметра
        public abstract double CalculatePerimeter();
        
        // Абстрактний метод обчислення площі
        public abstract double CalculateArea();
        
        // Спільний метод для виведення інформації
        public virtual string GetInfo()
        {
            return $"Фігура: {Name}\n" +
                   $"Периметр: {CalculatePerimeter():F2}\n" +
                   $"Площа: {CalculateArea():F2}";
        }
    }
    
    // Інтерфейс для трикутників
    public interface ITriangle
    {
        double SideA { get; set; }
        double SideB { get; set; }
        double SideC { get; set; }
        
        bool IsValidTriangle();
        string GetTriangleType();
    }
    
    // Клас "Трикутник", що успадковує абстрактний клас Shape 
    // та реалізує інтерфейс ITriangle
    public class Triangle : Shape, ITriangle
    {
        protected double sideA;
        protected double sideB;
        protected double sideC;
        
        public Triangle(double a, double b, double c)
        {
            sideA = a;
            sideB = b;
            sideC = c;
            name = "Трикутник";
        }
        
        public override string Name 
        { 
            get { return name; } 
            set { name = value; } 
        }
        
        public double SideA 
        { 
            get { return sideA; } 
            set { sideA = value; } 
        }
        
        public double SideB 
        { 
            get { return sideB; } 
            set { sideB = value; } 
        }
        
        public double SideC 
        { 
            get { return sideC; } 
            set { sideC = value; } 
        }
        
        public override double CalculatePerimeter()
        {
            return sideA + sideB + sideC;
        }
        
        public override double CalculateArea()
        {
            // Формула Герона
            double s = CalculatePerimeter() / 2;
            return Math.Sqrt(s * (s - sideA) * (s - sideB) * (s - sideC));
        }
        
        public bool IsValidTriangle()
        {
            return (sideA + sideB > sideC) && 
                   (sideA + sideC > sideB) && 
                   (sideB + sideC > sideA);
        }
        
        public virtual string GetTriangleType()
        {
            if (!IsValidTriangle())
                return "Недійсний трикутник";
                
            if (sideA == sideB && sideB == sideC)
                return "Рівносторонній";
            else if (sideA == sideB || sideB == sideC || sideA == sideC)
                return "Рівнобедрений";
            else
                return "Різносторонній";
        }
    }
    
    // Клас "Прямокутний трикутник"
    public class RightTriangle : Triangle
    {
        public RightTriangle(double cathetus1, double cathetus2) 
            : base(cathetus1, cathetus2, CalculateHypotenuse(cathetus1, cathetus2))
        {
            name = "Прямокутний трикутник";
        }
        
        // Конструктор з заданням всіх сторін
        public RightTriangle(double a, double b, double c) : base(a, b, c)
        {
            name = "Прямокутний трикутник";
        }
        
        private static double CalculateHypotenuse(double cathetus1, double cathetus2)
        {
            return Math.Sqrt(cathetus1 * cathetus1 + cathetus2 * cathetus2);
        }
        
        public double GetCathetus1()
        {
            // Припускаємо, що sideC - гіпотенуза
            return sideA;
        }
        
        public double GetCathetus2()
        {
            return sideB;
        }
        
        public double GetHypotenuse()
        {
            return sideC;
        }
        
        public bool IsRightTriangle()
        {
            double[] sides = { sideA, sideB, sideC };
            Array.Sort(sides);
            
            // Перевірка теореми Піфагора
            return Math.Abs(sides[0] * sides[0] + sides[1] * sides[1] - 
                           sides[2] * sides[2]) < 0.0001;
        }
        
        public override double CalculateArea()
        {
            // Для прямокутного трикутника площа = (катет1 * катет2) / 2
            if (IsRightTriangle())
            {
                double[] sides = { sideA, sideB, sideC };
                Array.Sort(sides);
                return (sides[0] * sides[1]) / 2;
            }
            return base.CalculateArea();
        }
        
        public override string GetTriangleType()
        {
            return "Прямокутний трикутник";
        }
        
        public override string GetInfo()
        {
            return base.GetInfo() + 
                   $"\nКатет 1: {GetCathetus1():F2}\n" +
                   $"Катет 2: {GetCathetus2():F2}\n" +
                   $"Гіпотенуза: {GetHypotenuse():F2}\n" +
                   $"Кут між катетами: 90°";
        }
    }
    
    // Головний клас програми
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Робота з прямокутними трикутниками ===\n");
            
            // Створення прямокутного трикутника за катетами
            RightTriangle rt1 = new RightTriangle(3, 4);
            Console.WriteLine(rt1.GetInfo());
            Console.WriteLine($"Чи є прямокутним: {rt1.IsRightTriangle()}\n");
            
            Console.WriteLine(new string('-', 50) + "\n");
            
            // Створення звичайного трикутника
            Triangle t1 = new Triangle(5, 6, 7);
            Console.WriteLine(t1.GetInfo());
            Console.WriteLine($"Тип трикутника: {t1.GetTriangleType()}\n");
            
            Console.WriteLine(new string('-', 50) + "\n");
            
            // Створення ще одного прямокутного трикутника
            RightTriangle rt2 = new RightTriangle(5, 12);
            Console.WriteLine(rt2.GetInfo());
            
            Console.WriteLine(new string('-', 50) + "\n");
            
            // Порівняння периметрів
            Console.WriteLine($"Периметр першого прямокутного трикутника: {rt1.CalculatePerimeter():F2}");
            Console.WriteLine($"Периметр другого прямокутного трикутника: {rt2.CalculatePerimeter():F2}");
            Console.WriteLine($"Периметр звичайного трикутника: {t1.CalculatePerimeter():F2}");
            
            Console.ReadKey();
        }
    }
}
