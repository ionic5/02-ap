namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class Node
    {
        public int GridX;
        public int GridY;

        public int GCost;
        public int HCost;
        public int FCost { get { return GCost + HCost; } }

        public bool IsWalkable;

        public Node Parent;

        public Node(bool isWalkable, int gridX, int gridY)
        {
            IsWalkable = isWalkable;
            GridX = gridX;
            GridY = gridY;
        }
    }
}
