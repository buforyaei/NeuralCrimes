using System;
using System.Linq;


namespace NeuralTest.Model
{
    public class TrainingModel
    {
        public DateTime Dates { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string Category { get; set; }
        public int TrueCrimeIndex { get; set; }
        public string Status { get; set; }

        public double NormalizedY { get; set; }
        public double NormalizedX { get; set; }
        public double NormalizedDayOfWeek { get; set; }
        public double NormalizedHour { get; set; }
        public double NormalizedDayOfMonth { get; set; }
        public double NormalizedMonth { get; set; }
        public double NormalizedDate { get; set; }
        public double[] CategoryVector { get; set; }

        public TrainingModel() { }
        public TrainingModel(DateTime dates, double x, double y,string category)
        {
            Dates = dates;
            DayOfWeek = dates.DayOfWeek;
            NormalizedDayOfWeek = (double)((int)DayOfWeek)/7;
            NormalizedMonth = (double)((int) Dates.Month)/12;
            NormalizedDayOfMonth = (double) ((int) Dates.Day)/31;
            NormalizedHour = (double) ((int) Dates.Hour)/24;
            Category = category;
            X = x;
            Y = y;
            CategoryVector = new Crimes().SearchCategoryVector(this);
            TrueCrimeIndex = EvaluateTrueCrimeIndex();
            Status = "UNKNOWN";
        }

        private int EvaluateTrueCrimeIndex()
        {
            if (CategoryVector.Count() != 39) return 0;
            for(int i=0;i<39;i++)
            {
                if (CategoryVector[i] == 1) return i;
            }
            return 0;
        }
    }

    public class Crimes
    {
        public Crimes()
        {

            ARSON =                     new double[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ASSAULT =                   new double[] { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            BADCHECKS =                 new double[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            BRIBERY =                   new double[] { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            BURGLARY =                  new double[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            DISORDERLYCONDUCT =         new double[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            DRIVINGUNDERTHEINFLUENCE =  new double[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            DRUGNARCOTIC =              new double[] { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            DRUNKENNESS =               new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            EMBEZZLEMENT =              new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            EXTORTION =                 new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FAMILYOFFENSES =            new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FORGERYCOUNTERFEITING =     new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            FRAUD =                     new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            GAMBLING =                  new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            KIDNAPPING =                new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            LARCENYTHEFT =              new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            LIQUORLAWS =                new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            LOITERING =                 new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            MISSINGPERSON =             new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            NONCRIMINAL =               new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            OTHEROFFENSES =             new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            PORNOGRAPHYOBSCENEMAT =     new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            PROSTITUTION =              new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            RECOVEREDVEHICLE =          new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ROBBERY =                   new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            RUNAWAY =                   new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            SECONDARYCODES =            new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            SEXOFFENSESFORCIBLE =       new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            SEXOFFENSESNONFORCIBLE =    new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            STOLENPROPERTY =            new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
            SUICIDE =                   new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
            SUSPICIOUSOCC =             new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
            TREA =                      new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
            TRESPASS =                  new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 };
            VANDALISM =                 new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
            VEHICLETHEFT =              new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 };
            WARRANTS =                  new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 };
            WEAPONLAWS =                new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }; 
        }

        public double[] ARSON { get; set; }
        public double[] ASSAULT { get; set; }
        public double[] BADCHECKS { get; set; }
        public double[] BRIBERY { get; set; }
        public double[] BURGLARY { get; set; }
        public double[] DISORDERLYCONDUCT { get; set; }
        public double[] DRIVINGUNDERTHEINFLUENCE { get; set; }
        public double[] DRUGNARCOTIC { get; set; }
        public double[] DRUNKENNESS { get; set; }
        public double[] EMBEZZLEMENT { get; set; }
        public double[] EXTORTION { get; set; }
        public double[] FAMILYOFFENSES { get; set; }
        public double[] FORGERYCOUNTERFEITING { get; set; }
        public double[] FRAUD { get; set; }
        public double[] GAMBLING { get; set; }
        public double[] KIDNAPPING { get; set; }
        public double[] LARCENYTHEFT { get; set; }
        public double[] LIQUORLAWS { get; set; }
        public double[] LOITERING { get; set; }
        public double[] MISSINGPERSON { get; set; }
        public double[] NONCRIMINAL { get; set; }
        public double[] OTHEROFFENSES { get; set; }
        public double[] PORNOGRAPHYOBSCENEMAT { get; set; }
        public double[] PROSTITUTION { get; set; }
        public double[] RECOVEREDVEHICLE { get; set; }
        public double[] ROBBERY { get; set; }
        public double[] RUNAWAY { get; set; }
        public double[] SECONDARYCODES { get; set; }
        public double[] SEXOFFENSESFORCIBLE { get; set; }
        public double[] SEXOFFENSESNONFORCIBLE { get; set; }
        public double[] STOLENPROPERTY { get; set; }
        public double[] SUICIDE { get; set; }
        public double[] SUSPICIOUSOCC { get; set; }
        public double[] TREA { get; set; }
        public double[] TRESPASS { get; set; }
        public double[] VANDALISM { get; set; }
        public double[] VEHICLETHEFT { get; set; }
        public double[] WARRANTS { get; set; }
        public double[] WEAPONLAWS { get; set; }



        public double[] SearchCategoryVector(TrainingModel t)
        {
            switch (t.Category)
            {
              
                case "ARSON": return ARSON;
                case "ASSAULT":return ASSAULT;
                case "BAD CHECKS": return BADCHECKS;
                case "BRIBERY": return BRIBERY;
                case "BURGLARY": return BURGLARY;
                case "DISORDERLY CONDUCT": return DISORDERLYCONDUCT;
                case "DRIVING UNDER THE INFLUENCE": return DRIVINGUNDERTHEINFLUENCE;
                case "DRUG/NARCOTIC": return DRUGNARCOTIC;
                case "DRUNKENNESS": return DRUNKENNESS;
                case "EMBEZZLEMENT": return EMBEZZLEMENT;
                case "EXTORTION": return EXTORTION;
                case "FAMILY OFFENSES": return FAMILYOFFENSES;
                case "FORGERY/COUNTERFEITING": return FORGERYCOUNTERFEITING;
                case "FRAUD": return FRAUD;
                case "GAMBLING": return GAMBLING;
                case "KIDNAPPING": return KIDNAPPING;
                case "LARCENY/THEFT": return LARCENYTHEFT;
                case "LIQUOR LAWS": return LIQUORLAWS;
                case "LOITERING": return LOITERING;
                case "MISSING PERSON": return MISSINGPERSON;
                case "NON-CRIMINAL": return NONCRIMINAL;
                case "OTHER OFFENSES": return OTHEROFFENSES;
                case "PORNOGRAPHY/OBSCENE MAT": return PORNOGRAPHYOBSCENEMAT;
                case "PROSTITUTION": return PROSTITUTION;
                case "RECOVERED VEHICLE": return RECOVEREDVEHICLE;
                case "ROBBERY": return ROBBERY;
                case "RUNAWAY": return RUNAWAY;
                case "SECONDARY CODES": return SECONDARYCODES;
                case "SEX OFFENSES FORCIBLE": return SEXOFFENSESFORCIBLE;
                case "SEX OFFENSES NON FORCIBLE": return SEXOFFENSESNONFORCIBLE;
                case "STOLEN PROPERTY": return STOLENPROPERTY;
                case "SUICIDE": return SUICIDE;
                case "SUSPICIOUS OCC": return SUSPICIOUSOCC;
                case "TREA": return TREA;
                case "TRESPASS": return TRESPASS;
                case "VANDALISM": return VANDALISM;
                case "VEHICLE THEFT": return VEHICLETHEFT;
                case "WARRANTS": return WARRANTS;
                case "WEAPON LAWS": return WEAPONLAWS;

                default:
                    return OTHEROFFENSES;
            }
             
        }

    }
}
