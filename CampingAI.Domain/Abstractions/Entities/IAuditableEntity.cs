namespace CampingAI.Domain.Abstractions.Entities;
public interface IAuditableEntity {
    void Updated();

    void Created();

}