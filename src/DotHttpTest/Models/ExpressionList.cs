using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotHttpTest.Models
{
    public class ExpressionList
    {
        private readonly List<Expression> mExpressions = new List<Expression>();

        public static ExpressionList Create(string text)
        {
            var expressions = new ExpressionList();
            expressions.Add(new Text(text));
            return expressions;
        }

        public byte[] ToByteArray(Encoding encoding, TestStatus? status)
        {
            var sb = new List<byte>();
            foreach (var expression in mExpressions)
            {
                sb.AddRange(expression.ToByteArray(encoding, status));
            }
            return sb.ToArray();
        }
        public string ToString(TestStatus? status)
        {
            var sb = new StringBuilder();
            foreach (var expression in mExpressions)
            {
                sb.Append(expression.ToString(status));
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var expression in mExpressions)
            {
                sb.Append(expression.ToString());
            }
            return sb.ToString();
        }

        internal void AddRange(IEnumerable<Expression> expressions)
        {
            mExpressions.AddRange(expressions);
        }
        internal void Add(ExpressionList expression)
        {
            AddRange(expression.mExpressions);
        }
        internal void Add(Expression expression)
        {
            mExpressions.Add(expression);
        }
        internal void Add(string text)
        {
            mExpressions.Add(new Text(text));
        }
    }
}
