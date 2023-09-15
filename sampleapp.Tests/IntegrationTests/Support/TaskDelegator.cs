using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sampleapp.Tests.IntegrationTests.Support
{
	public class TaskDelegator
	{
		private readonly List<Func<Task>> _taskList;

		public TaskDelegator()
		{
			_taskList = new List<Func<Task>>();
		}

		public TaskDelegator(List<Func<Task>> existingTaskList)
		{
			_taskList = existingTaskList;
		}

		public void AddTask(Func<Task> task)
		{
			_taskList.Add(task);
		}

		public void AddTasks(IEnumerable<Func<Task>> tasks)
		{
			_taskList.AddRange(tasks);
		}

		public async Task<IEnumerable<string>> RunTasksAsync()
		{
			var exceptions = new List<string>();

			if (_taskList.Count == 0)
			{
				Console.Out.WriteLine("TaskDelegator has 0 tasks. Returning null.");
				return null;
			}

			Console.Out.WriteLine("TaskDelegator is now running through its task list...");

			foreach (var task in _taskList)
			{
				try
				{
					await task();
				}
				catch (AggregateException ex)
				{
					foreach (var inner in ex.InnerExceptions)
					{
						exceptions.Add($"Caught AggregateException in Main at {DateTime.UtcNow}: " + inner.Message);
					}
				}
				catch (Exception ex)
				{
					exceptions.Add($"Caught Exception in Main at {DateTime.UtcNow}: " + ex.Message);
				}
			}

			Console.WriteLine("TaskDelegator is done.");
			return exceptions;
		}
	}
}
