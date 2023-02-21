using System;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.Services.ValidationHelperService
{
    public class MeasurementValidator : IMeasurementValidator
    {

        public bool IsMeasurementValid(Measurement measurement, Plant plant)
        {
            if(measurement == null || plant == null)
            {
                return false;
            }

            foreach (var limit in plant.MeasurementValueLimits)
            {
                var type = limit.Type;
                if (!IsMeasurementTypeValid(type))
                {
                    return false;
                }

                var valueOfType = measurement.getValueByType(type);
                if(valueOfType == null)
                {
                    break;
                }
                if(valueOfType > limit.UpperLimit || valueOfType < limit.LowerLimit)
                {
                    return false;
                }
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
    }
}

