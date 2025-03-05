using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Commitments;

namespace SFA.DAS.Forecasting.Application.Commitments.Handlers;

public interface IDeletePledgeApplicationCommitmentsHandler
{
    Task Handle(long employerAccountId);
}
public class DeletePledgeApplicationCommitmentsHandler : IDeletePledgeApplicationCommitmentsHandler
{
    private readonly IEmployerCommitmentRepository _repository;

    public DeletePledgeApplicationCommitmentsHandler(IEmployerCommitmentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(long employerAccountId)
    {
        await _repository.DeletePledgeApplicationCommitmentsForSendingEmployer(employerAccountId);
    }
}