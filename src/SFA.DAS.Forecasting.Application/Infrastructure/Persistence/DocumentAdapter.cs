using SFA.DAS.Forecasting.Models;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Persistence;

/// <summary>
/// This class is used to allow storing and querying of multiple types of classes in the same azure cosmos collection
/// </summary>
/// <typeparam name="T"></typeparam>
public class DocumentAdapter<T> where T: class, IDocument
{
    public T Document { get; set; }

    // ReSharper disable once InconsistentNaming
    public string id { get ; set ; }

    // ReSharper disable once InconsistentNaming
    public string type { get => Document.GetType().FullName; set { } }

    protected DocumentAdapter() { } //for serialization
    public DocumentAdapter(string id, T document)
    {
        this.id = id;
        Document = document;
    }
}