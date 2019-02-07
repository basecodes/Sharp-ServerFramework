using System;
using System.Collections.Generic;

namespace Ssc.SscPatterns {
    public class TreeNode<T> where T : IComparable<T> {
        public List<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();

        public T Node { get; set; }

        public TreeNode<T> Add(T child) {
            var newNode = new TreeNode<T> {Node = child};
            Children.Add(newNode);
            return newNode;
        }

        public void Remove(T child) {
            foreach (var treeNode in Children)
                if (treeNode.Node.CompareTo(child) == 0) {
                    Children.Remove(treeNode);
                    return;
                }
        }

        public static void Display(TreeNode<T> node, int indentation) {
            var line = new string('-', indentation);
            Console.WriteLine(line + " " + node.Node);

            node.Children.ForEach(n => Display(n, indentation + 1));
        }
    }
}