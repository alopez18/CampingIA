using CampingAI.Domain.Abstractions.ValueObjects;

namespace CampingAI.Domain.ValueObjects;
public class EmployeeUserNameVO : SimpleStringRequiredValueObject<EmployeeUserNameVO> {
    public EmployeeUserNameVO(string value) : base(value) {

    }
}