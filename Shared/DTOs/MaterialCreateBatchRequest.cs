using Shared.DTOs.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record MaterialCreateBatchRequest(List<MaterialCreateRequest> Materials);
}
