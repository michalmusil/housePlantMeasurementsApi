using System;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.Services.ValidationHelperService
{
    public class MeasurementValidator : IMeasurementValidator
    {

        public bool IsMeasurementValid(Measurement measurement, Plant plant, Device device)
        {
            if(measurement == null || plant == null || device == null)
            {
                return false;
            }

            // Device must be activated, must be assigned to user and must be assigned to mentioned plant
            if (!device.IsActive || device.UserId == null || device.PlantId != plant.Id)
            {
                return false;
            }

            // Measurements plantId and deviceId must be set to mentioned plant and device
            if(measurement.PlantId != plant.Id || measurement.DeviceId != device.Id)
            {
                return false;
            }

            return true;
        }

        public bool IsMeasurementTypeValid(MeasurementType? measurementType)
        {
            if(measurementType == null)
            {
                return false;
            }
            return Enum.IsDefined(typeof(MeasurementType), measurementType);
        }

        public bool AreMeasurementLimitsValid(List<MeasurementValueLimit> measurementValueLimits)
        {
            foreach(var limit in measurementValueLimits)
            {
                if (!IsMeasurementTypeValid(limit.Type))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsMeasurementWithinLimits(Measurement measurement, List<MeasurementValueLimit> limits)
        {
            foreach (var limit in limits)
            {
                var type = limit.Type;
                if (!IsMeasurementTypeValid(type))
                {
                    continue;
                }
                var valueOfType = measurement.getValueByType(type);
                if (valueOfType == null)
                {
                    continue;
                }
                if (valueOfType > limit.UpperLimit || valueOfType < limit.LowerLimit)
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<MeasurementType> GetInvalidMeasurementTypes(Measurement measurement, List<MeasurementValueLimit> limits)
        {
            var invalidMeasurementTypes = new List<MeasurementType>();
            foreach (var limit in limits)
            {
                var type = limit.Type;
                if (!IsMeasurementTypeValid(type))
                {
                    continue;
                }
                var valueOfType = measurement.getValueByType(type);
                if (valueOfType == null)
                {
                    continue;
                }
                if (valueOfType > limit.UpperLimit || valueOfType < limit.LowerLimit)
                {
                    invalidMeasurementTypes.Add(type);
                }
            }

            return invalidMeasurementTypes;
        }
    }
}

