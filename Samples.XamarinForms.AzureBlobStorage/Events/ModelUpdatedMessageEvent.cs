using Prism.Events;
using Xamarin.Forms;

namespace Samples.XamarinForms.AzureBlobStorage.Events
{
    public enum ModelUpdateEvent
    {
        Created,
        Updated,
        Deleted
    }

    public class ModelUpdatedMessageEvent<T> : PubSubEvent<ModelUpdatedMessageResult<T>>
    {
        public static void Publish(T updatedModel, ModelUpdateEvent updateEvent)
        {
            var updateResult = new ModelUpdatedMessageResult<T>
            {
                UpdatedModel = updatedModel,
                UpdateEvent = updateEvent
            };
            var eventMessenger = DependencyService.Get<IEventAggregator>(DependencyFetchTarget.GlobalInstance);
            eventMessenger.GetEvent<ModelUpdatedMessageEvent<T>>().Publish(updateResult);
        }
    }

    public class ModelUpdatedMessageResult<T>
    {
        public T UpdatedModel { get; set; }

        public ModelUpdateEvent UpdateEvent { get; set; }
    }
}