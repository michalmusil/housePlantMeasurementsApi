using System;
namespace HousePlantMeasurementsApi.Services.ValidationHelperService
{
    public class ValidationHelperService: IValidationHelperService
    {
        public bool IsBetweenBoundries(double? start, double testedNumber, double? end)
        {
            if (start != null && testedNumber < start)
            {
                return false;
            }
            if (end != null && testedNumber > end)
            {
                return false;
            }
            return true;
        }
    }
}

