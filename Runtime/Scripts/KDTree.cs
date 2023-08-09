using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XRC.Students.SU2023.IN06.Yuan
{
    /// <summary>
    /// Constructor for a KD tree along with functions to build the tree and find the nearest neighbour in the tree recursively. 
    /// </summary>
    public class KDTree
    {
        private Node root;

        private class Node
        {
            public Vector3 position;
            public Collider collider;
            public Node left;
            public Node right;

            public Node(Vector3 position, Collider collider)
            {
                this.position = position;
                this.collider = collider;
                left = null;
                right = null;
            }

        }

        /// <summary>
        /// Constructor for a KD tree. 
        /// </summary>
        public KDTree()
        {
            root = null;
        }
        /// <summary>
        /// Build a KD tree from a list of colliders. 
        /// </summary>
        /// <param name="colliders"></param>
        public void Build(List<Collider> colliders)
        {

            root = BuildRecursive(colliders, 0);
        }

        /// <summary>
        /// A helper function to recursively build the KD tree. 
        /// </summary>
        /// <param name="colliderList"> A list of colliders to build the tree from </param>
        /// <param name="depth"> The depth of tree thus far. </param>
        /// <returns></returns>
        private Node BuildRecursive(List<Collider> colliderList, int depth)
        {
            if (colliderList.Count == 0)
                return null;

            int axis = depth % 3;
            colliderList.Sort((a, b) => a.transform.position[axis].CompareTo(b.transform.position[axis]));

            int medianIndex = colliderList.Count / 2;
            Node node = new Node(colliderList[medianIndex].transform.position, colliderList[medianIndex]);

            node.left = BuildRecursive(colliderList.GetRange(0, medianIndex), depth + 1);
            node.right = BuildRecursive(colliderList.GetRange(medianIndex + 1, colliderList.Count - medianIndex - 1),
                depth + 1);

            return node;
        }

        /// <summary>
        /// Find the closest object to a given target. 
        /// </summary>
        /// <param name="target"> The given target. </param>
        /// <returns></returns>
        public Collider FindNearestNeighbor(Vector3 target)
        {
            Node nearest = FindNearestNeighborRecursive(root, target, 0);
            return nearest.collider;
        }
        /// <summary>
        /// A helper function to find the closest object recursively. 
        /// </summary>
        /// <param name="node"> A node in the KD tree. </param>
        /// <param name="target"> The given target. </param>
        /// <param name="depth"> The current depth of the tree. </param>
        /// <returns></returns>
        private Node FindNearestNeighborRecursive(Node node, Vector3 target, int depth)
        {
            if (node == null)
                return null;

            int axis = depth % 3;

            if (target[axis] < node.position[axis])
            {
                Node nextNode = FindNearestNeighborRecursive(node.left, target, depth + 1);
                if (nextNode != null && Vector3.Distance(target, nextNode.position) <
                    Vector3.Distance(target, node.position))
                    node = nextNode;
            }
            else
            {
                Node nextNode = FindNearestNeighborRecursive(node.right, target, depth + 1);
                if (nextNode != null && Vector3.Distance(target, nextNode.position) <
                    Vector3.Distance(target, node.position))
                    node = nextNode;
            }

            return node;
        }
    }
}
