using System;
using HousePlantMeasurementsApi.Data.Entities;
using HousePlantMeasurementsApi.Data.Enums;

namespace HousePlantMeasurementsApi.Services.ValidationHelperService
{
    public interface IMeasurementValidator
    {
        public bool IsMeasurementValid(Measurement measurement, Plant plant);

        public bool IsMeasurementTypeValid(MeasurementType? measurementType);

        public bool AreMeasurementLimitsValid(List<MeasurementValueLimit> measurementValueLimits);
    }
}

