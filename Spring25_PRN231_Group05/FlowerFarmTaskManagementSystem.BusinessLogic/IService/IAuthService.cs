using FlowerFarmTaskManagementSystem.BusinessObject.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerFarmTaskManagementSystem.BusinessLogic.IService
{
	public interface IAuthService
	{
		Task<AuthResponseDTO> LoginAsync(AuthRequestDTO authRequestDTO);

	}
}
