using System;
namespace HousePlantMeasurementsApi.Services.ValidationHelperService
{
    public interface IValidationHelperService
    {
        public bool IsBetweenBoundries(double? start, double testedNumber, double? end);
    }
}

