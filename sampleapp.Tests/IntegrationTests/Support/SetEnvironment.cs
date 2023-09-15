namespace sampleapp.Tests.IntegrationTests.Support
{
	public class SetEnvironment
	{
		private static Lazy<SetEnvironment> _lazy;

		public TaskDelegator Delegator { get; }

		public static SetEnvironment Instance
		{
			get
			{
				if (_lazy == null)
				{
					throw new NullReferenceException("SetEnvironment was not initialized.");
				}

				return _lazy.Value;
			}
		}

		public static void Init()
		{
			if (_lazy != null)
			{
				throw new InvalidOperationException("SetEnvironment is already initialized.");
			}

			_lazy = new Lazy<SetEnvironment>(() => new SetEnvironment());
		}

		private SetEnvironment()
		{
			Delegator = new TaskDelegator();

			// Other things that could be created to setup connections could be created/loaded here
			// Environment variables
		}
	}
}
