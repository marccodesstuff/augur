using Mapster;
using MiniGrc.Application.DTOs;
using MiniGrc.Domain.Entities;
using MiniGrc.Domain.Enums;

namespace MiniGrc.Application.Mappings;

public static class MappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Control, ControlDto>.NewConfig()
            .Map(dest => dest.Framework, src => src.Framework.ToString())
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.EvidenceCount, src => src.Evidence.Count)
            .Map(dest => dest.ApprovedEvidenceCount, src => src.Evidence.Count(e => e.Status == EvidenceStatus.Approved));

        TypeAdapterConfig<Domain.Entities.Evidence, EvidenceDto>.NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToString());

        TypeAdapterConfig<Finding, FindingDto>.NewConfig()
            .Map(dest => dest.Severity, src => src.Severity.ToString())
            .Map(dest => dest.RemediationTasks, src => src.RemediationTasks);

        TypeAdapterConfig<RemediationTask, RemediationTaskDto>.NewConfig()
            .Map(dest => dest.Priority, src => src.Priority.ToString());

        TypeAdapterConfig<Risk, RiskDto>.NewConfig()
            .Map(dest => dest.Severity, src => src.Severity.ToString());
    }
}
