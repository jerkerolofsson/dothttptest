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

        /// <summary>
        /// This returns the expressions as a byte array.
        /// It is invoked when creating the request
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public byte[] ToByteArray(Encoding encoding, TestStatus? status, StageWorkerState? stageWorkerState)
        {
            var sb = new List<byte>();
            foreach (var expression in mExpressions)
            {
                sb.AddRange(expression.ToByteArray(encoding, status, stageWorkerState));
            }
            return sb.ToArray();
        }
        public string ToString(TestStatus? status, StageWorkerState? stageWorkerState)
        {
            var sb = new StringBuilder();
            foreach (var expression in mExpressions)
            {
                sb.Append(expression.ToString(status, stageWorkerState));
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
