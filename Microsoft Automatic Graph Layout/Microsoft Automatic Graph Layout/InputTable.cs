namespace Microsoft_Automatic_Graph_Layout
{
    public class InputTable
    {
        public string Operation { get; set; }
        public string BeforeOperations { get; set; }
        public int Time { get; set; }

        public InputTable() : this("","",-1)
        {
        }

        public InputTable(string Operation, string BeforeOperations, int Time)
        {
            this.Operation = Operation;
            this.BeforeOperations = BeforeOperations;
            this.Time = Time;
        }

        public override string ToString()
        {
            return Operation + " " + BeforeOperations + " " + Time;
        }
    }
}