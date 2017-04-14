using System;
using System.IO;
using System.Threading.Tasks;
using AdaSDK;

namespace AdaBot.Models.Interfaces
{
    public interface IDataService : IDisposable
    {
        Task<PersonDto[]> RecognizePersonsAsync(Stream picture);
        Task UpdatePersonInformation(PersonUpdateDto personUpdateDto);
    }
}
