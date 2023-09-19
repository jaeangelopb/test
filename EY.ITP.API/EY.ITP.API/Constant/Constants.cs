namespace EY.ITP.API.Constant
{
    public static class Constants
    {
        #region Schema
        public const string Schema_Core = "Core";
        public const string Schema_Common = "Common";
        public const string Schema_Reporting = "Reporting";
        public const string Schema_WorkPaper = "WorkPaper";
        #endregion

        #region Stored Procedure
        public const string SP_Param_Mode_In = "in";
        public const string SP_Param_Mode_Out = "out";
        #endregion

        #region FileValidation
        public const string FileValidation_SP_BulkSave_SPName = "sp_FileValidation_BulkSave";
        public const string FileValidation_SP_BulkSave_TableType_ParamName = "@tbl_FileValidation";
        public const string FileValidation_SP_BulkSave_TableType_ParamType = "tbl_FileValidation_Save";

        public const string FileValidation_SP_BulkUpdate_SPName = "sp_FileValidation_BulkUpdate";
        public const string FileValidation_SP_BulkUpdate_TableType_ParamName = "@tbl_FileValidation_Update";
        public const string FileValidation_SP_BulkUpdate_TableType_ParamType = "tbl_FileValidation_Update";

        public const string FileValidation_VW_List_VWName = "vw_FileValidation_List";
        #endregion

        #region PAAdjustments
        public const string PAAdjustments_SP_BulkSave_SPName = "sp_PAAdjustments_BulkSave";
        public const string PAAdjustments_SP_BulkSave_TableType_ParamName = "@tbl_PAAdjustmentsSave";
        public const string PAAdjustments_SP_BulkSave_TableType_ParamType = "tbl_PAAdjustmentsSave";

        public const string PAAdjustments_VW_List_VWName = "vw_PAAdjustments_Workpaper";
        #endregion
    }
}
