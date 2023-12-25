using DAL.Models;
using Framework.DatabaseCommand.DatabaseCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.AdministratorRepository
{
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly IDatabaseCommand<User> _dbCommand;
        public AdministratorRepository(IDatabaseCommand<User> dbCommand)
        {
            _dbCommand = dbCommand;
        }
    }
}
