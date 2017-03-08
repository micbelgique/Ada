using System.IO;
using System.Threading.Tasks;
using AdaSDK;

namespace AdaW10.Design
{
    public class DesignDataService : Models.Interfaces.IDataService
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task<PersonDto[]> RecognizePersonsAsync(Stream picture)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdatePersonInformation(PersonUpdateDto personUpdateDto)
        {
            throw new System.NotImplementedException();
        }
    }
}
