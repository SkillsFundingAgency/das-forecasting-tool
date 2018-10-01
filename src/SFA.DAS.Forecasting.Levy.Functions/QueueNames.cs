namespace SFA.DAS.Forecasting.Levy.Functions
{
    public static class QueueNames
    {
        public const string ProcessDeclaration = "forecasting-levy-process-declaration";
        public const string ValidateDeclaration = "forecasting-levy-validate-declaration";
	    public const string ValidateLevyDeclarationNoProjection = "forecasting-levy-validate-declaration-no-projection";
		public const string StoreLevyDeclaration = "forecasting-levy-store-declaration";
	    public const string StoreLevyDeclarationNoProjection = "forecasting-levy-store-declaration-no-projection";
		public const string AllowProjection = "forecasting-levy-allow-projection";
        public const string GenerateProjections = "forecasting-projections-generate-projections";
    }
}