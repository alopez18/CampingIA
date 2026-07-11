
namespace CampingAI.Infra.Abstractions;
public class ModelExtractor<TModel> {
    //NOTAS: Reutilizo el modelado de entity framework para extraer los nombres de los campos de la tabla. Lo hacemos Singleton para que solo se genere una vez para.

    readonly List<string> _fieldNames = new List<string>();


    public ModelExtractor() {
        _fieldNames = ExtractFieldNames();
    }

    public List<string> FieldNames => _fieldNames;

    public string GetFieldNamesForSql() {
        return string.Join(", ", _fieldNames);
    }

    public string GetTableNameForSql() {
        return typeof(TModel).Name;
    }

    private List<string> ExtractFieldNames() {
        var fieldNames = new List<string>();
        var properties = typeof(TModel).GetProperties();
        foreach (var property in properties) {
            fieldNames.Add(property.Name);
        }
        return fieldNames;
    }
}
