namespace ClassMethodAnalyzer.Model
{
    public class ClassMethods
    {
        public List<string> Methods { get; }
        public string Constructor { get; }

        public ClassMethods(List<string> methods, string constructor)
        {
            Methods = methods;
            Constructor = constructor;
        }
    }
}
