using Flee.PublicTypes;

using Random = System.Random;

namespace SCKRM.Flee
{
    public static class FleeManager
    {
        public static ExpressionContext expressionContext { get; } = new ExpressionContext();

        static FleeManager()
        {
            expressionContext.Variables.Add(nameof(MathUtility.e), MathUtility.e);
            expressionContext.Variables.Add(nameof(MathUtility.pi), MathUtility.pi);
            expressionContext.Variables.Add(nameof(MathUtility.deg2Rad), MathUtility.deg2RadDouble);
            expressionContext.Variables.Add(nameof(MathUtility.rad2Deg), MathUtility.rad2DegDouble);
            expressionContext.Variables.Add(nameof(MathUtility.epsilonFloatWithAccuracy), MathUtility.epsilonFloatWithAccuracy);

            expressionContext.Variables.Add("ninf", double.NegativeInfinity);
            expressionContext.Variables.Add("inf", double.PositiveInfinity);
            expressionContext.Variables.Add(nameof(double.Epsilon), double.Epsilon);
            expressionContext.Variables.Add(nameof(double.NaN), double.NaN);

            expressionContext.Variables.Add(nameof(Random), new Random());

            expressionContext.Imports.AddType(typeof(MathUtility));
        }

        public static T Calculate<T>(string expression) => expressionContext.CompileGeneric<T>(expression).Evaluate();
        public static object Calculate(string expression) => expressionContext.CompileDynamic(expression).Evaluate();

        public static T Calculate<T>(this ExpressionContext expressionContext, string expression) => expressionContext.CompileGeneric<T>(expression).Evaluate();
        public static object Calculate(this ExpressionContext expressionContext, string expression) => expressionContext.CompileDynamic(expression).Evaluate();
    }
}