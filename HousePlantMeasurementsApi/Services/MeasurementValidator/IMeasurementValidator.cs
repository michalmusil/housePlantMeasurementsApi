using System;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.Services.ValidationHelperService
{
    public interface IMeasurementValidator
    {
        public bool IsMeasurementValid(Measurement measurement, Plant plant, Device device);

        public bool IsMeasurementTypeValid(MeasurementType? measurementType);

        public bool AreMeasurementLimitsValid(List<MeasurementValueLimit> measurementValueLimits);

        public bool IsMeasurementWithinLimits(Measurement measurement, List<MeasurementValueLimit> limits);

        public IEnumerable<MeasurementType> GetInvalidMeasurementTypes(Measurement measurement, List<MeasurementValueLimit> limits);
    }
}

