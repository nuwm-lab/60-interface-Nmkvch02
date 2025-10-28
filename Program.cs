using System;
using System.IO;

namespace GeometryShapes
{
    // ============= Interfaces =============
    
    /// <summary>
    /// Інтерфейс для логування
    /// </summary>
    public interface ILogger
    {
        void LogInfo(string message);
    }
    
    /// <summary>
    /// Інтерфейс для трикутників
    /// </summary>
    public interface ITriangle
    {
        double SideA { get; set; }
        double SideB { get; set; }
        double SideC { get; set; }
        
        bool IsValidTriangle();
        string GetTriangleType();
    }
    
    // ============= Loggers =============
    
    /// <summary>
    /// Логер для виведення в консоль
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss}: {message}");
        }
    }
    
    /// <summary>
    /// Логер для запису у файл
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        
        public FileLogger(string filePath = "geometry.log")
        {
            _filePath = filePath;
        }
        
        public void LogInfo(string message)
        {
            try
            {
                File.AppendAllText(_filePath, 
                    $"[LOG] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запису в файл: {ex.Message}");
            }
        }
    }
    
    // ============= Abstract Classes =============
    
    /// <summary>
    /// Абстрактний клас для геометричних фігур
    /// </summary>
    public abstract class Shape
    {
        private const double Tolerance = 0.0001;
        private string _name;
        protected readonly ILogger _logger;
        
        protected Shape(string name, ILogger logger = null)
        {
            _name = name;
            _logger = logger ?? new ConsoleLogger();
            _logger.LogInfo($"Створено об'єкт: {_name}");
        }
        
        public virtual string Name 
        { 
            get => _name; 
            set => _name = value; 
        }
        
        /// <summary>
        /// Обчислює периметр фігури
        /// </summary>
        public abstract double CalculatePerimeter();
        
        /// <summary>
        /// Обчислює площу фігури
        /// </summary>
        public abstract double CalculateArea();
        
        /// <summary>
        /// Перевизначений метод ToString для виведення інформації
        /// </summary>
        public override string ToString()
        {
            return $"Фігура: {Name}\n" +
                   $"Периметр: {CalculatePerimeter():F2}\n" +
                   $"Площа: {CalculateArea():F2}";
        }
        
        /// <summary>
        /// Порівняння дробових чисел з допуском
        /// </summary>
        protected static bool AreEqual(double a, double b)
        {
            return Math.Abs(a - b) < Tolerance;
        }
        
        protected static double Tolerance => 0.0001;
    }
    
    // ============= Classes =============
    
    /// <summary>
    /// Клас для роботи з трикутниками
    /// </summary>
    public class Triangle : Shape, ITriangle
    {
        private double _sideA;
        private double _sideB;
        private double _sideC;
        
        public Triangle(double a, double b, double c, ILogger logger = null) 
            : base("Трикутник", logger)
        {
            ValidateSides(a, b, c);
            _sideA = a;
            _sideB = b;
            _sideC = c;
            
            if (!IsValidTriangle())
            {
                throw new ArgumentException(
                    "Сторони не утворюють дійсний трикутник (не виконується нерівність трикутника)");
            }
            
            _logger.LogInfo($"Трикутник створено: a={a:F2}, b={b:F2}, c={c:F2}");
        }
        
        public double SideA 
        { 
            get => _sideA;
            set 
            { 
                ValidateSide(value, nameof(SideA));
                _sideA = value;
            }
        }
        
        public double SideB 
        { 
            get => _sideB;
            set 
            { 
                ValidateSide(value, nameof(SideB));
                _sideB = value;
            }
        }
        
        public double SideC 
        { 
            get => _sideC;
            set 
            { 
                ValidateSide(value, nameof(SideC));
                _sideC = value;
            }
        }
        
        public override double CalculatePerimeter()
        {
            double perimeter = _sideA + _sideB + _sideC;
            _logger.LogInfo($"Обчислено периметр: {perimeter:F2}");
            return perimeter;
        }
        
        public override double CalculateArea()
        {
            if (!IsValidTriangle())
            {
                throw new InvalidOperationException(
                    "Неможливо обчислити площу недійсного трикутника");
            }
            
            // Формула Герона
            double s = CalculatePerimeter() / 2;
            double area = Math.Sqrt(s * (s - _sideA) * (s - _sideB) * (s - _sideC));
            
            _logger.LogInfo($"Обчислено площу: {area:F2}");
            return area;
        }
        
        public bool IsValidTriangle()
        {
            return (_sideA + _sideB > _sideC) && 
                   (_sideA + _sideC > _sideB) && 
                   (_sideB + _sideC > _sideA);
        }
        
        public virtual string GetTriangleType()
        {
            if (!IsValidTriangle())
                return "Недійсний трикутник";
                
            if (AreEqual(_sideA, _sideB) && AreEqual(_sideB, _sideC))
                return "Рівносторонній";
            else if (AreEqual(_sideA, _sideB) || 
                     AreEqual(_sideB, _sideC) || 
                     AreEqual(_sideA, _sideC))
                return "Рівнобедрений";
            else
                return "Різносторонній";
        }
        
        public override string ToString()
        {
            return base.ToString() + $"\nТип: {GetTriangleType()}";
        }
        
        private void ValidateSide(double side, string paramName)
        {
            if (side <= 0)
                throw new ArgumentOutOfRangeException(paramName, 
                    "Сторона трикутника повинна бути більшою за нуль");
        }
        
        private void ValidateSides(double a, double b, double c)
        {
            ValidateSide(a, nameof(a));
            ValidateSide(b, nameof(b));
            ValidateSide(c, nameof(c));
        }
    }
    
    /// <summary>
    /// Клас для роботи з прямокутними трикутниками
    /// </summary>
    public class RightTriangle : Triangle
    {
        private readonly double _cathetus1;
        private readonly double _cathetus2;
        private readonly double _hypotenuse;
        
        /// <summary>
        /// Конструктор за двома катетами (автоматично обчислює гіпотенузу)
        /// </summary>
        public RightTriangle(double cathetus1, double cathetus2, ILogger logger = null) 
            : base(cathetus1, cathetus2, CalculateHypotenuse(cathetus1, cathetus2), logger)
        {
            _cathetus1 = cathetus1;
            _cathetus2 = cathetus2;
            _hypotenuse = CalculateHypotenuse(cathetus1, cathetus2);
            Name = "Прямокутний трикутник";
            
            _logger.LogInfo($"Прямокутний трикутник: катет1={cathetus1:F2}, " +
                          $"катет2={cathetus2:F2}, гіпотенуза={_hypotenuse:F2}");
        }
        
        /// <summary>
        /// Конструктор з заданням всіх трьох сторін (з перевіркою прямокутності)
        /// </summary>
        public RightTriangle(double a, double b, double c, ILogger logger = null) 
            : base(a, b, c, logger)
        {
            Name = "Прямокутний трикутник";
            
            // Визначаємо, які сторони є катетами, а яка - гіпотенузою
            double[] sides = { a, b, c };
            Array.Sort(sides);
            
            if (!CheckRightTriangle(sides[0], sides[1], sides[2]))
            {
                throw new ArgumentException(
                    "Задані сторони не утворюють прямокутний трикутник");
            }
            
            _cathetus1 = sides[0];
            _cathetus2 = sides[1];
            _hypotenuse = sides[2];
            
            _logger.LogInfo($"Прямокутний трикутник створено з трьох сторін");
        }
        
        public double Cathetus1 => _cathetus1;
        public double Cathetus2 => _cathetus2;
        public double Hypotenuse => _hypotenuse;
        
        private static double CalculateHypotenuse(double cathetus1, double cathetus2)
        {
            return Math.Sqrt(cathetus1 * cathetus1 + cathetus2 * cathetus2);
        }
        
        /// <summary>
        /// Перевіряє, чи є трикутник прямокутним за теоремою Піфагора
        /// </summary>
        public bool IsRightTriangle()
        {
            double[] sides = { SideA, SideB, SideC };
            Array.Sort(sides);
            return CheckRightTriangle(sides[0], sides[1], sides[2]);
        }
        
        private static bool CheckRightTriangle(double a, double b, double c)
        {
            // c - найбільша сторона (гіпотенуза)
            double sumOfSquares = a * a + b * b;
            double hypotenuseSquared = c * c;
            return Math.Abs(sumOfSquares - hypotenuseSquared) < Tolerance;
        }
        
        public override double CalculateArea()
        {
            // Для прямокутного трикутника: площа = (катет1 × катет2) / 2
            double area = (_cathetus1 * _cathetus2) / 2;
            _logger.LogInfo($"Обчислено площу прямокутного трикутника: {area:F2}");
            return area;
        }
        
        public override string GetTriangleType()
        {
            return "Прямокутний трикутник";
        }
        
        public override string ToString()
        {
            return $"Фігура: {Name}\n" +
                   $"Периметр: {CalculatePerimeter():F2}\n" +
                   $"Площа: {CalculateArea():F2}\n" +
                   $"Катет 1: {_cathetus1:F2}\n" +
                   $"Катет 2: {_cathetus2:F2}\n" +
                   $"Гіпотенуза: {_hypotenuse:F2}\n" +
                   $"Кут між катетами: 90°";
        }
    }
    
    // ============= Program =============
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            try
            {
                Console.WriteLine("=== Демонстрація роботи з трикутниками ===\n");
                
                // Створення логерів
                ILogger consoleLogger = new ConsoleLogger();
                ILogger fileLogger = new FileLogger("geometry.log");
                
                // Демонстрація використання інтерфейсу ITriangle
                Console.WriteLine("--- Демонстрація поліморфізму через інтерфейс ---\n");
                
                ITriangle triangle1 = new Triangle(5, 6, 7, consoleLogger);
                ITriangle triangle2 = new RightTriangle(3, 4, fileLogger);
                
                DisplayTriangleInfo(triangle1);
                Console.WriteLine();
                DisplayTriangleInfo(triangle2);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Демонстрація поліморфізму через абстрактний клас Shape
                Console.WriteLine("--- Демонстрація поліморфізму через Shape ---\n");
                
                Shape[] shapes = 
                {
                    new Triangle(8, 10, 12, consoleLogger),
                    new RightTriangle(5, 12, consoleLogger),
                    new RightTriangle(6, 8, 10, fileLogger)
                };
                
                DisplayShapesInfo(shapes);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Порівняння характеристик
                Console.WriteLine("--- Порівняння трикутників ---\n");
                CompareTriangles(shapes);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Демонстрація обробки помилок
                Console.WriteLine("--- Демонстрація валідації ---\n");
                TestValidation();
                
                Console.WriteLine("\nПрограма завершила роботу успішно!");
                Console.WriteLine("Логи збережено у файл geometry.log");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nКритична помилка: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Виводить інформацію про трикутник через інтерфейс
        /// </summary>
        static void DisplayTriangleInfo(ITriangle triangle)
        {
            Console.WriteLine($"Тип: {triangle.GetTriangleType()}");
            Console.WriteLine($"Сторони: a={triangle.SideA:F2}, " +
                            $"b={triangle.SideB:F2}, c={triangle.SideC:F2}");
            Console.WriteLine($"Дійсність: {triangle.IsValidTriangle()}");
        }
        
        /// <summary>
        /// Виводить інформацію про масив фігур через поліморфізм
        /// </summary>
        static void DisplayShapesInfo(Shape[] shapes)
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                Console.WriteLine($"Фігура #{i + 1}:");
                Console.WriteLine(shapes[i].ToString());
                Console.WriteLine();
            }
        }
        
        /// <summary>
        /// Порівнює характеристики трикутників
        /// </summary>
        static void CompareTriangles(Shape[] shapes)
        {
            double maxPerimeter = 0;
            double maxArea = 0;
            Shape largestByPerimeter = null;
            Shape largestByArea = null;
            
            foreach (var shape in shapes)
            {
                double perimeter = shape.CalculatePerimeter();
                double area = shape.CalculateArea();
                
                if (perimeter > maxPerimeter)
                {
                    maxPerimeter = perimeter;
                    largestByPerimeter = shape;
                }
                
                if (area > maxArea)
                {
                    maxArea = area;
                    largestByArea = shape;
                }
            }
            
            Console.WriteLine($"Найбільший периметр: {maxPerimeter:F2} " +
                            $"({largestByPerimeter.Name})");
            Console.WriteLine($"Найбільша площа: {maxArea:F2} " +
                            $"({largestByArea.Name})");
        }
        
        /// <summary>
        /// Тестує валідацію даних
        /// </summary>
        static void TestValidation()
        {
            try
            {
                Console.WriteLine("Спроба створити трикутник з від'ємною стороною...");
                var invalid1 = new Triangle(-3, 4, 5);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"✓ Перехоплено помилку: {ex.Message}\n");
            }
            
            try
            {
                Console.WriteLine("Спроба створити недійсний трикутник (1, 2, 10)...");
                var invalid2 = new Triangle(1, 2, 10);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Перехоплено помилку: {ex.Message}\n");
            }
            
            try
            {
                Console.WriteLine("Спроба створити непрямокутний трикутник як RightTriangle...");
                var invalid3 = new RightTriangle(3, 4, 6);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✓ Перехоплено помилку: {ex.Message}");
            }
        }
    }
}
