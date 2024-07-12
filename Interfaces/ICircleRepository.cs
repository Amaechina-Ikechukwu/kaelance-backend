using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kallum.DTOS.CircleDto;
using Kallum.Helper;

namespace Kallum.Interfaces
{
    public interface ICircleRepository
    {
        public Task<CircleResponseDto?> AddCommitment(string bankId, Guid circleId, double percentage);
        public Task<CircleResponseDto?> InitiatePersonalWithdrawalFromCircle(string bankId, Guid circleId);
        public Task<CircleResponseDto?> InitiateCircleWithdrawal(string bankId, Guid circleId);
        public Task<List<CreateCircleActivity>> ActivityHistory(Guid circleId);
        public Task<CircleResponseDto?> UpdateFriendsList(Guid circleId, string bankId, string type);

        public Task<CircleResponseDto?> CommitToCircle(Guid circleId, string username, double percentage);
        public Task<CircleResponseDto?> InitiateWidthdrawl(Guid circleId, string username);
    }
}