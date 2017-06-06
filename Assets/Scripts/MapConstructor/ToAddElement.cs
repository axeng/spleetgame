using System;
namespace Assets.Script
{
	public class ToAddElement
	{
		public string[] buts;
		public string element;
		public ExecType type;
		public ExecutorType executorType;
		public bool tag;
		public ToAddElement(string[] buts, string element, ExecType type, ExecutorType executorType, bool tag = true)
		{
			this.buts = buts;
			this.element = element;
			this.type = type;
			this.executorType = executorType;
			this.tag = tag;
		}
	}
}
