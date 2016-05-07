using System;

namespace UsingRocks
{
	public sealed class ServiceUser
	{
		public ServiceUser(IService service)
		{
			if(service == null)
			{
				throw new ArgumentNullException(nameof(service));
			}

			this.Id = service.GetId();
		}

		public int Id { get; }
	}
}
