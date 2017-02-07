using System;
using System.IO;
using System.Threading.Tasks;
using AdaBridge;

namespace AdaW10.Models.Interfaces
{
    public interface IDataService : IDisposable
    {
        Task<PersonDto[]> RecognizePersonsAsync(Stream picture);
        Task UpdatePersonInformation(PersonUpdateDto personUpdateDto);
    }
}
