using System;
using System.IO;

namespace GeometryShapes
{
    // ============= ILogger.cs =============
    
    /// <summary>
    /// Інтерфейс для логування повідомлень
    /// </summary>
    public interface ILogger
    {
        void LogInfo(string message);
    }
    
    // ============= ConsoleLogger.cs =============
    
    /// <summary>
    /// Логер для виведення повідомлень в консоль
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss}: {message}");
        }
    }
    
    // ============= FileLogger.cs =============
    
    /// <summary>
    /// Логер для запису повідомлень у файл з підтримкою IDisposable
    /// </summary>
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _filePath;
        private StreamWriter _writer;
        private bool _disposed = false;
        
        public FileLogger(string filePath = "geometry.log")
        {
            _filePath = filePath;
            _writer = new StreamWriter(_filePath, append: true) { AutoFlush = true };
        }
        
        public void LogInfo(string message)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileLogger));
                
            try
            {
                _writer.WriteLine($"[LOG] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запису в файл: {ex.Message}");
                throw;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writer?.Dispose();
                }
                _disposed = true;
            }
        }
        
        ~FileLogger()
        {
            Dispose(false);
        }
    }
    
    // ============= ITriangle.cs =============
    
    /// <summary>
    /// Інтерфейс для роботи з трикутниками
    /// </summary>
    public interface ITriangle
    {
        double SideA { get; }
        double SideB { get; }
        double SideC { get; }
        
        bool IsValidTriangle();
        string GetTriangleType();
        void UpdateSides(double a, double b, double c);
    }
    
    // ============= IPrintable.cs =============
    
    /// <summary>
    /// Інтерфейс для об'єктів, що можуть бути виведені
    /// </summary>
    public interface IPrintable
    {
        string GetDetailedInfo();
        void Print();
    }
    
    // ============= Shape.cs =============
    
    /// <summary>
    /// Абстрактний базовий клас для геометричних фігур
    /// </summary>
    public abstract class Shape
    {
        protected const double Tolerance = 0.0001;
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
        protected static bool AreEqual(double a, double b) => 
            Math.Abs(a - b) < Tolerance;
    }
    
    // ============= Polygon.cs =============
    
    /// <summary>
    /// Абстрактний клас для багатокутників
    /// </summary>
    public abstract class Polygon : Shape
    {
        protected Polygon(string name, ILogger logger = null) 
            : base(name, logger)
        {
        }
        
        /// <summary>
        /// Повертає кількість сторін багатокутника
        /// </summary>
        public abstract int GetSidesCount();
        
        /// <summary>
        /// Перевіряє, чи є багатокутник опуклим
        /// </summary>
        public abstract bool IsConvex();
    }
    
    // ============= Triangle.cs =============
    
    /// <summary>
    /// Клас для роботи з трикутниками
    /// </summary>
    public class Triangle : Polygon, ITriangle, IPrintable
    {
        private double _sideA;
        private double _sideB;
        private double _sideC;
        
        public Triangle(double a, double b, double c, ILogger logger = null) 
            : base("Трикутник", logger)
        {
            ValidateSide(a, "Сторона A");
            ValidateSide(b, "Сторона B");
            ValidateSide(c, "Сторона C");
            
            if (!CheckTriangleInequality(a, b, c))
            {
                throw new ArgumentException(
                    $"Сторони {a:F2}, {b:F2}, {c:F2} не утворюють дійсний трикутник");
            }
            
            _sideA = a;
            _sideB = b;
            _sideC = c;
            
            _logger.LogInfo($"Трикутник створено: a={a:F2}, b={b:F2}, c={c:F2}");
        }
        
        public double SideA => _sideA;
        public double SideB => _sideB;
        public double SideC => _sideC;
        
        /// <summary>
        /// Атомарне оновлення всіх трьох сторін з валідацією
        /// </summary>
        public void UpdateSides(double a, double b, double c)
        {
            ValidateSide(a, "Сторона A");
            ValidateSide(b, "Сторона B");
            ValidateSide(c, "Сторона C");
            
            if (!CheckTriangleInequality(a, b, c))
            {
                throw new ArgumentException(
                    $"Нові сторони {a:F2}, {b:F2}, {c:F2} не утворюють дійсний трикутник");
            }
            
            _sideA = a;
            _sideB = b;
            _sideC = c;
            
            _logger.LogInfo($"Сторони оновлено: a={a:F2}, b={b:F2}, c={c:F2}");
        }
        
        public override int GetSidesCount() => 3;
        
        public override bool IsConvex() => true; // Всі трикутники опуклі
        
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
        
        public bool IsValidTriangle() => 
            CheckTriangleInequality(_sideA, _sideB, _sideC);
        
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
        
        public string GetDetailedInfo()
        {
            return $"{ToString()}\nТип: {GetTriangleType()}\n" +
                   $"Кількість сторін: {GetSidesCount()}\n" +
                   $"Опуклість: {(IsConvex() ? "Так" : "Ні")}";
        }
        
        public void Print()
        {
            Console.WriteLine(GetDetailedInfo());
        }
        
        public override string ToString()
        {
            return base.ToString() + $"\nТип: {GetTriangleType()}";
        }
        
        private static void ValidateSide(double side, string sideName)
        {
            if (side <= 0)
                throw new ArgumentOutOfRangeException(sideName, 
                    $"{sideName} повинна бути більшою за нуль (отримано: {side})");
        }
        
        private static bool CheckTriangleInequality(double a, double b, double c)
        {
            return (a + b > c) && (a + c > b) && (b + c > a);
        }
    }
    
    // ============= RightTriangle.cs =============
    
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
                    $"Задані сторони {a:F2}, {b:F2}, {c:F2} не утворюють прямокутний трикутник");
            }
            
            _cathetus1 = sides[0];
            _cathetus2 = sides[1];
            _hypotenuse = sides[2];
            
            _logger.LogInfo($"Прямокутний трикутник створено з трьох сторін");
        }
        
        public double Cathetus1 => _cathetus1;
        public double Cathetus2 => _cathetus2;
        public double Hypotenuse => _hypotenuse;
        
        private static double CalculateHypotenuse(double cathetus1, double cathetus2) =>
            Math.Sqrt(cathetus1 * cathetus1 + cathetus2 * cathetus2);
        
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
        
        public override string GetTriangleType() => "Прямокутний трикутник";
        
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
    
    // ============= Circle.cs =============
    
    /// <summary>
    /// Клас для роботи з колами (демонстрація іншої фігури)
    /// </summary>
    public class Circle : Shape, IPrintable
    {
        private double _radius;
        
        public Circle(double radius, ILogger logger = null) 
            : base("Коло", logger)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius), 
                    "Радіус повинен бути більшим за нуль");
                
            _radius = radius;
            _logger.LogInfo($"Коло створено: радіус={radius:F2}");
        }
        
        public double Radius => _radius;
        
        public override double CalculatePerimeter()
        {
            double perimeter = 2 * Math.PI * _radius;
            _logger.LogInfo($"Обчислено довжину кола: {perimeter:F2}");
            return perimeter;
        }
        
        public override double CalculateArea()
        {
            double area = Math.PI * _radius * _radius;
            _logger.LogInfo($"Обчислено площу кола: {area:F2}");
            return area;
        }
        
        public string GetDetailedInfo()
        {
            return $"{ToString()}\nРадіус: {_radius:F2}\n" +
                   $"Діаметр: {2 * _radius:F2}";
        }
        
        public void Print()
        {
            Console.WriteLine(GetDetailedInfo());
        }
    }
    
    // ============= Program.cs =============
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            FileLogger fileLogger = null;
            
            try
            {
                Console.WriteLine("=== Демонстрація роботи з геометричними фігурами ===\n");
                
                // Створення логерів
                ILogger consoleLogger = new ConsoleLogger();
                fileLogger = new FileLogger("geometry.log");
                
                // Демонстрація використання інтерфейсу ITriangle
                Console.WriteLine("--- 1. Поліморфізм через інтерфейс ITriangle ---\n");
                DemonstrateITriangle(consoleLogger, fileLogger);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Демонстрація поліморфізму через абстрактний клас Shape
                Console.WriteLine("--- 2. Поліморфізм через абстрактний клас Shape ---\n");
                DemonstrateShapePolymorphism(consoleLogger, fileLogger);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Демонстрація інтерфейсу IPrintable
                Console.WriteLine("--- 3. Поліморфізм через інтерфейс IPrintable ---\n");
                DemonstrateIPrintable(consoleLogger, fileLogger);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Демонстрація абстрактного класу Polygon
                Console.WriteLine("--- 4. Робота з абстрактним класом Polygon ---\n");
                DemonstratePolygon(consoleLogger);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Демонстрація атомарного оновлення сторін
                Console.WriteLine("--- 5. Оновлення сторін трикутника ---\n");
                DemonstrateUpdateSides(consoleLogger);
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                // Демонстрація обробки помилок
                Console.WriteLine("--- 6. Валідація та обробка помилок ---\n");
                TestValidation();
                
                Console.WriteLine("\n" + new string('=', 60) + "\n");
                
                Console.WriteLine("✓ Програма завершила роботу успішно!");
                Console.WriteLine("✓ Логи збережено у файл geometry.log");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Критична помилка: {ex.Message}");
            }
            finally
            {
                // Демонстрація деструктора через IDisposable
                fileLogger?.Dispose();
                Console.WriteLine("\n[Деструктор] FileLogger знищено та ресурси звільнено");
            }
        }
        
        static void DemonstrateITriangle(ILogger consoleLogger, ILogger fileLogger)
        {
            ITriangle triangle1 = new Triangle(5, 6, 7, consoleLogger);
            ITriangle triangle2 = new RightTriangle(3, 4, fileLogger);
            
            DisplayTriangleInfo(triangle1, "Трикутник 1");
            Console.WriteLine();
            DisplayTriangleInfo(triangle2, "Трикутник 2");
        }
        
        static void DemonstrateShapePolymorphism(ILogger consoleLogger, ILogger fileLogger)
        {
            Shape[] shapes = 
            {
                new Triangle(8, 10, 12, consoleLogger),
                new RightTriangle(5, 12, consoleLogger),
                new Circle(7.5, fileLogger)
            };
            
            Console.WriteLine("Обхід масиву фігур через базовий тип Shape:\n");
            
            for (int i = 0; i < shapes.Length; i++)
            {
                Console.WriteLine($"Фігура #{i + 1}: {shapes[i].Name}");
                Console.WriteLine($"  Периметр: {shapes[i].CalculatePerimeter():F2}");
                Console.WriteLine($"  Площа: {shapes[i].CalculateArea():F2}");
                Console.WriteLine();
            }
            
            CompareShapes(shapes);
        }
        
        static void DemonstrateIPrintable(ILogger consoleLogger, ILogger fileLogger)
        {
            IPrintable[] printables = 
            {
                new Triangle(6, 8, 10, consoleLogger),
                new RightTriangle(3, 4, fileLogger),
                new Circle(5, consoleLogger)
            };
            
            Console.WriteLine("Виведення через інтерфейс IPrintable:\n");
            
            foreach (var item in printables)
            {
                item.Print();
                Console.WriteLine(new string('-', 40));
            }
        }
        
        static void DemonstratePolygon(ILogger logger)
        {
            Polygon[] polygons = 
            {
                new Triangle(5, 7, 9, logger),
                new RightTriangle(6, 8, logger)
            };
            
            Console.WriteLine("Робота з багатокутниками:\n");
            
            foreach (var polygon in polygons)
            {
                Console.WriteLine($"{polygon.Name}:");
                Console.WriteLine($"  Кількість сторін: {polygon.GetSidesCount()}");
                Console.WriteLine($"  Опуклий: {(polygon.IsConvex() ? "Так" : "Ні")}");
                Console.WriteLine($"  Периметр: {polygon.CalculatePerimeter():F2}");
                Console.WriteLine($"  Площа: {polygon.CalculateArea():F2}");
                Console.WriteLine();
            }
        }
        
        static void DemonstrateUpdateSides(ILogger logger)
        {
            Triangle triangle = new Triangle(3, 4, 5, logger);
            
            Console.WriteLine("Початкові сторони:");
            Console.WriteLine($"  a={triangle.SideA:F2}, b={triangle.SideB:F2}, c={triangle.SideC:F2}");
            Console.WriteLine($"  Периметр: {triangle.CalculatePerimeter():F2}");
            Console.WriteLine($"  Площа: {triangle.CalculateArea():F2}\n");
            
            triangle.UpdateSides(5, 12, 13);
            
            Console.WriteLine("Після оновлення сторін:");
            Console.WriteLine($"  a={triangle.SideA:F2}, b={triangle.SideB:F2}, c={triangle.SideC:F2}");
            Console.WriteLine($"  Периметр: {triangle.CalculatePerimeter():F2}");
            Console.WriteLine($"  Площа: {triangle.CalculateArea():F2}");
        }
        
        static void DisplayTriangleInfo(ITriangle triangle, string name)
        {
            Console.WriteLine($"{name}:");
            Console.WriteLine($"  Тип: {triangle.GetTriangleType()}");
            Console.WriteLine($"  Сторони: a={triangle.SideA:F2}, b={triangle.SideB:F2}, c={triangle.SideC:F2}");
            Console.WriteLine($"  Дійсність: {triangle.IsValidTriangle()}");
        }
        
        static void CompareShapes(Shape[] shapes)
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
            
            Console.WriteLine("Порівняння фігур:");
            Console.WriteLine($"  Найбільший периметр: {maxPerimeter:F2} ({largestByPerimeter.Name})");
            Console.WriteLine($"  Найбільша площа: {maxArea:F2} ({largestByArea.Name})");
        }
        
        static void TestValidation()
        {
            try
            {
                Console.WriteLine("Тест 1: Від'ємна сторона...");
                var invalid1 = new Triangle(-3, 4, 5);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"  ✓ Перехоплено: {ex.ParamName} - {ex.Message}\n");
            }
            
            try
            {
                Console.WriteLine("Тест 2: Недійсний трикутник (1, 2, 10)...");
                var invalid2 = new Triangle(1, 2, 10);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ✓ Перехоплено: {ex.Message}\n");
            }
            
            try
            {
                Console.WriteLine("Тест 3: Непрямокутний трикутник як RightTriangle...");
                var invalid3 = new RightTriangle(3, 4, 6);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ✓ Перехоплено: {ex.Message}\n");
            }
            
            try
            {
                Console.WriteLine("Тест 4: Оновлення на недійсні сторони...");
                var triangle = new Triangle(3, 4, 5);
                triangle.UpdateSides(1, 2, 20);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"  ✓ Перехоплено: {ex.Message}\n");
            }
            
            try
            {
                Console.WriteLine("Тест 5: Від'ємний радіус кола...");
                var invalidCircle = new Circle(-5);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"  ✓ Перехоплено: {ex.ParamName} - {ex.Message}");
            }
        }
    }
}
