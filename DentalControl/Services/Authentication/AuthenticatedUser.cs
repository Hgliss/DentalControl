using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalControl.Services.Authentication;

public record AuthenticatedUser
(
    int Id,
    string Username,
    string FullName,
    string RoleName);
