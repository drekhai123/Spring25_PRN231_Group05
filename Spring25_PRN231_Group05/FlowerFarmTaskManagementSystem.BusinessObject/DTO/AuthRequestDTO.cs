using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessObject.DTO
{
	public class AuthRequestDTO
	{
		public string UserNameOrEmail { get; set; }
		public string Password { get; set; }
	}
}
