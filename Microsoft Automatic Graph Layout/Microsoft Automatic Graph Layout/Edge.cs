namespace Microsoft_Automatic_Graph_Layout
{
    public class Edge
    {
        public string vertex1;
        public string vertex2;
        public string edgeLabel;

        public Edge(string v1, string e, string v2)
        {
            vertex1 = v1;
            vertex2 = v2;
            edgeLabel = e;
        }
        public override string ToString()
        {
            return vertex1 + " " + edgeLabel + " " + vertex2;
        }
    }
}