using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

/*namespace SnakeSharp
{
    public class Node<T>
    {
        public Node(T x, T y)
        {
            const int Data = 1;
            X = x;
            Y = y;
        }
        public int Data { get; set; }//как это всё инициализировать?
        public T X { get; set; }
        public T Y { get; set; }
        public Node<T> Previous { get; set; }
        public Node<T> Next { get; set; }
    }


    public class Body<T> : IEnumerable<T>
    {
        Node<T> head; // головной элемент
        Node<T> tail; // хвостовой элемент
        int count;  // количество элементов в теле змеи

        // добавление первого элемента
        public void AddFirst(T data)
        {
            Node<T> node = new Node<T>(x, y);
            Node<T> temp = head;
            node.Next = temp;
            head = node;
            if (count == 0)
                tail = head;
            else
                temp.Previous = node;
            count++;
        }

        

        public int Count { get { return count; } }
        public bool IsEmpty { get { return count == 0; } }

        public void Clear()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public bool Contains(T data)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Data.Equals(data))
                    return true;
                current = current.Next;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()//интерфейс(?) для использования foreach
        {
            return ((IEnumerable)this).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Node<T> current = head;
            while (current != null)
            {
                yield return current.X;
                yield return current.Y;
                current = current.Next;
            }
        }

        public IEnumerable<T> BackEnumerator()
        {
            Node<T> current = tail;
            while (current != null)
            {
                yield return current.X;
                yield return current.Y;
                current = current.Previous;
            }
        }
    }




    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter size of the field");
            int n = Convert.ToInt32(Console.ReadLine());
            int[,] numbers = new int[n,n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write($"{numbers[i, j]} \t");
                }
                Console.WriteLine();
            }
        }
    }
}*/

namespace SnakeSharp
{
    /* todo задания:
        1. Поправить комменты
        2. Сделать игру перезапускаемой (без закрытия/открытия консоли)
        3. Добавить ещё один вид яблок, который будет добавлять 3 ячейки к змейке (как - на твоё усмотрение)
        4. Добавить победное сообщение, когда змейка всё поле заполняет
        5. Иногда на поворотах змейка некорректно отображается (пропадает символ из тела) - попробуй исправить (в самый последний момент, когда всё уже сделано)
     */
    struct Node//класс всех элементов
    {
        public int X { get; set; }
        public int Y { get; set; }
        public char S { get; set; }

        /*todo знаю, что спиздил, но всё же это пиздец
         в районе 200 строки продолжение
         почитать, что такое касты и какие они бывают, что такое кортеж. Написать в вк    
        */
        public static implicit operator Node((int, int, char) value)
        {
            return new Node { X = value.Item1, Y = value.Item2, S = value.Item3 };
        }
        public static bool operator ==(Node a, Node b)//перегрузка оператора ==, чтобы он корректно работал с Node
        {
            if (a.X == b.X && a.Y == b.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool operator !=(Node a, Node b)//перегрузка оператора !=, чтобы он корректно работал с Node
        {
            if (a.X != b.X || a.Y != b.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void WriteNode(char _ch)
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(_ch);
        }

        public void Write()
        {
            WriteNode(S);
        }
        public void Clear()
        {
            WriteNode(' ');
        }
    }

    class Borders//класс границ
    {
        private char S;
        private List<Node> wall = new List<Node>();

        public Borders(int X, int Y, char S)
        {
            this.S = S;
            WriteVertical(0, Y);
            WriteVertical(X, Y);
            WriteHorizontal(X, 0);
            WriteHorizontal(X, Y);
        }

        private void WriteVertical(int X, int Y)
        {
            for (int i = 0; i < Y; i++)
            {
                // todo в следующей строчке создаётся кортеж, а затем через каст (реализованный в самом верху) создаётся нода
                // нахуя? кто бы знал... Почему бы не создавать сразу ноду? избавься везде от этих кортежей
                Node p = (X, i, S);
                p.Write();
                wall.Add(p);
            }
        }

        private void WriteHorizontal(int X, int Y)
        {
            for (int i = 0; i < X; i++)
            {
                Node p = (i, Y, S);
                p.Write();
                wall.Add(p);
            }
        }

        public bool IsHit(Node p)
        {
            foreach (var w in wall)
            {
                if (p == w)
                {
                    return true;
                }
            }
            return false;
        }
    }

    enum Orientation
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    class Body
    {
        public List<Node> body;
        public Orientation orientation;
        public int step = 1;
        public Node tail;
        public Node head;
        bool turn = true;

        public Body(int X, int Y, int length)
        {
            orientation = Orientation.RIGHT;

            body = new List<Node>();
            for (int i = X - length; i < X; i++)
            {
                Node p = (i, Y, '1');
                body.Add(p);

                p.Write();
            }
        }

        public Node GetHead()
        {
            return body.Last();
        }

        public void Move()
        {
            //todo много где некст нода помещается в head, но фактически это же не head, это нода следующая после головы
            // очень сильно сбивает восприятие, переименуй как-нибудь, nodeAfterHead например или что-то подобное
            // хотя тоже хуйня, в какую сторону AfterHead? непонятно. подумай короче
            head = GetNextNode();
            body.Add(head);
            tail = body.First();
            body.Remove(tail);
            tail.Clear();
            head.Write();
            turn = true;
        }

        //todo метод позволяет съесть абсолютно любую ноду, кажется не очень логичным, может проверять, что едим фрукт?
        public bool Eat(Node p)
        {
            head = GetNextNode();
            if (head == p)
            {
                body.Add(head);
                head.Write();
                return true;
            }
            return false;
        }

        public Node GetNextNode()
        {
            Node p = GetHead();
            switch (orientation)
            {
                case Orientation.UP:
                    p.Y -= step;
                    break;
                case Orientation.DOWN:
                    p.Y += step;
                    break;
                case Orientation.LEFT:
                    p.X -= step;
                    break;
                case Orientation.RIGHT:
                    p.X += step;
                    break;
            }
            return p;
        }

        public void Turning(ConsoleKey key)
        {
            if (turn)
            {
                switch (orientation)
                {
                    case Orientation.LEFT:
                    case Orientation.RIGHT:
                        if (key == ConsoleKey.UpArrow)
                            orientation = Orientation.UP;
                        else if (key == ConsoleKey.DownArrow)
                            orientation = Orientation.DOWN;
                        break;
                    case Orientation.UP:
                    case Orientation.DOWN:
                        if (key == ConsoleKey.RightArrow)
                            orientation = Orientation.RIGHT;
                        else if (key == ConsoleKey.LeftArrow)
                            orientation = Orientation.LEFT;
                        break;
                }
                turn = false;
            }

        }

        public bool IsHit(Node p)//проверка на столкновение с самой змейкой
        {
            for (int i = body.Count - 2; i > 0; i--)
            {
                if (body[i] == p)
                {
                    return true;
                }
            }
            return false;
        }
    }

    // todo знаю, что ты поменял спизженное название, но не суть
    // фактически, тут же не яблоко описано, а некий создатель яблок, так что название гавно
    // название x и y тут тоже и не очень (и в других местах). X и y это ж какие-то координаты, а ты тут (и не тут)
    // подразумеваешь ширину и высоту поля (либо максимальные координаты)
    // Однобуквенные названия (как S) преимущественно гавно, избавляйся от этого наследия университетов.
    // Даже назвать appleChar будет значительно понятнее и прозрачнее.
    // и посмотри, в каком стиле переменные надо называть и поправь везде + нажимай почаще alt+Enter там где подчеркивает линией,
    // там будут предлагаться варианты исправления косяков. Например приватные переменные, локальные и аргументы принято с маленькой буквы называть (а у тебя большая)
    class Apple
    {
        int X;
        int Y;
        char S;

        Random rng = new Random();
        public Node food { get; private set; }

        public Apple(int X, int Y, char S)
        {
            this.X = X;
            this.Y = Y;
            this.S = S;
        }

        public void CreateApple()
        {
            food = (rng.Next(2, X - 2), rng.Next(2, Y - 2), S);
            food.Write();
        }
    }

    class Game
    {
        // todo сделай поменьше пабрацки, заебался туда-сюда бегать
        static readonly int X = 60;
        static readonly int Y = 30;

        static Borders borders;
        static Body body;
        static Apple apple;
        static Timer time;

        static void Main()
        {
            Console.SetWindowSize(X + 1, Y + 1);
            Console.SetBufferSize(X + 1, Y + 1);
            Console.Title = "Snake Game";
            Console.CursorVisible = false;

            borders = new Borders(X, Y, '0');
            body = new Body(X / 2, Y / 2, 3);
            apple = new Apple(X, Y, '@');
            apple.CreateApple();

            time = new Timer(Movement, null, 0, 200);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    body.Turning(key.Key);
                }
            }
        }

        static void Movement(object obj)//класс движения с проверками на столкновение со стенами и яблоком
        {
            if (borders.IsHit(body.GetHead()) || body.IsHit(body.GetHead()))
            {
                time.Change(0, Timeout.Infinite);
                End();
            }
            else if (body.Eat(apple.food))
            {
                apple.CreateApple();
            }
            else
            {
                body.Move();
            }
        }
        private static void End()//не доделано, хочу перекомпоновать main так, чтобы можно было использовать этот класс для продолжения игры
        {
            Console.WriteLine("Gameover\n");
        }

    }
}
