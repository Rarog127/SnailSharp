using System;
using System.Collections.Generic;
using System.Collections;

namespace SnakeSharp
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
}
