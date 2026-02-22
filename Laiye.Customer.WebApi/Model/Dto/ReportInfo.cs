using Google.Protobuf.WellKnownTypes;

namespace Laiye.Customer.WebApi.Model
{
    public class ReportInfo
    {
        //流程编号
        public string id { get; set; }
        //租户编号
        public string  tenantId { get; set; }
        //项目编号
        public string projectId { get; set; }
        //流程名称
        public string name { get; set; }
        //人工工时
        public double laborCost { get; set; } 
       //流程价值
        public double flowValue { get; set; }
        //创建人
        public string creatorCode { get; set; }
        //创建时间
        public Timestamp ? createTime { get; set; }
        //资源地址
        public string url { get; set; }

        public string updatorCode { get; set; }
        //创建时间
        public Timestamp? updateTime { get; set; }
        //状态
        public int status { get; set; }
        //
        public string license { get; set; }
        //
        public string orderId { get; set; }
        //
        public long begin { get; set; }

        public long end { get; set; }

        public string ip { get; set; }

        public int type { get; set; }

    }

    public class WorkLisenceInfo
    {
        public DateTime startDate { get; set; }
        public DateTime expireDate { get; set; }

        public long itemId { get; set; }

    }

    public class ReportAddWorkerInfo
    {
        public string id { get; set; }        //租户编号
        public string tenantId { get; set; }        //项目编号
        public string projectId { get; set; }
        public string license { get; set; }
        public string orderId { get; set; }        //

        public long begin { get; set; }
        public long end { get; set; } 

        public string ip { get; set; }
        public string name { get; set; }
        public int type { get; set; }
   
        public string creatorCode { get; set; }
        public long createTime { get; set; }
    }

    public class ReportStatusWorkerInfo
    {
        public string id { get; set; }        //租户编号
        public string tenantId { get; set; }        //项目编号
        public string projectId { get; set; }
        public int      status { get; set; }
       // public string orderId { get; set; }
    }
        public class ReportModifyWorkerInfo
    {
        public string id { get; set; }        //租户编号
        public string tenantId { get; set; }        //项目编号
        public string projectId { get; set; }
        public string license { get; set; }
        public string orderId { get; set; }        
        public long begin { get; set; }
        public long end { get; set; }
        public string ip { get; set; }
        public string name { get; set; }
        public int type { get; set; }
        public int status { get;set; }
        public string creatorCode { get; set; }
        public long createTime { get; set; }
        public string updatorCode { get; set; }
        public long updateTime { get; set; }

    }

    public class ReportAddTaskInfo
    {
        public string id { get; set; }        //租户编号
        public string tenantId { get; set; }        //项目编号
        public string projectId { get; set; }
        public string flowId { get; set; }
        public string name { get; set; }

        public int status { get; set; }
        public long createTime { get; set; }
        public string creatorCode { get; set; }
    }

    public class ReportSuccessTaskInfo
    {
        public string id { get; set; }        //租户编号
        public string tenantId { get; set; }        //项目编号
        public string projectId { get; set; }
        public string flowId { get; set; }
        //public string name { get; set; }
        public int status { get; set; }
        //public long updateTime { get; set; }
        //public long createTime { get; set; }
        ////public string creatorCode { get; set; }

        public float  duration { get; set; }

        public long startTime { get; set; }

        public long endTime { get; set; }

        public long robotId { get; set; }
    }

    public class ReportDeleteTaskInfo
    {
        public string id { get; set; }        //租户编号
        public string tenantId { get; set; }        //项目编号
        public string projectId { get; set; }
        public string flowId { get; set; }
        public string name { get; set; }
        public int status { get; set; }

        public long createTime { get; set; }
        public string creatorCode { get; set; }
        public long updateTime { get; set; }
        //public long createTime { get; set; }
        public string updatorCode { get; set; }
        //public string creatorCode { get; set; }

    }

    public class ReportAddFlowInfo
    {
        //流程编号
        public string id { get; set; }
        //租户编号
        public string tenantId { get; set; }
        //项目编号
        public string projectId { get; set; }
        //流程名称
        public string name { get; set; }
        //人工工时
        public double laborCost { get; set; }
        //流程价值
        public double flowValue { get; set; }

        public string strategy { get; set; }

        //创建人
        public string creatorCode { get; set; }
        //创建时间
        public long createTime { get; set; }
        //资源地址
        public string url { get; set; }
    }

    public class ReportAddFlowInfo2
    {
        //流程编号
        public string id { get; set; }
        //租户编号
        public string tenantId { get; set; }
        //项目编号
        public string projectId { get; set; }
        //流程名称
        public string name { get; set; }
        //人工工时
        public double laborCost { get; set; }
        //流程价值
        public double flowValue { get; set; }

        public string strategy { get; set; }

        //创建人
        public string creatorCode { get; set; }
        //创建时间
        public long createTime { get; set; }
        //资源地址
        public string url { get; set; }
        public string originalFlowId { get; set; }
    }



    public class ReportModifyFlowInfo
    {
        //流程编号
        public string id { get; set; }
        //租户编号
        public string tenantId { get; set; }
        //项目编号
        public string projectId { get; set; }
        //流程名称
        public string name { get; set; }
        //人工工时
        public double laborCost { get; set; }
        //流程价值
        public double flowValue { get; set; }

        public string strategy { get; set; }

        //资源地址
        public string url { get; set; }

        public string creatorCode { get; set; }
        //创建时间
        public long createTime { get; set; }

        public string updatorCode { get; set; }
        //创建时间
        public long updateTime { get; set; }
    }

    public class ReportModifyFlowInfo3
    {
        //流程编号
        public string id { get; set; }
        //租户编号
        public string tenantId { get; set; }
        //项目编号
        public string projectId { get; set; }
        //流程名称
        public string name { get; set; }

        //共享流程
        public int shareType { get; set; }


        //人工工时
        public double laborCost { get; set; }
        //流程价值
        public double flowValue { get; set; }

        public string strategy { get; set; }

        //资源地址
        public string url { get; set; }

        public string creatorCode { get; set; }
        //创建时间
        public long createTime { get; set; }

        public string updatorCode { get; set; }
        //创建时间
        public long updateTime { get; set; }
    }


    public class ReportModifyFlowInfo4
    {
        //流程编号
        public string id { get; set; }
        //租户编号
        public string tenantId { get; set; }
        //项目编号
        public string projectId { get; set; }
        //流程名称
        public string name { get; set; }

        //共享流程
        public int shareType { get; set; }


        //人工工时
        public double laborCost { get; set; }
        //流程价值
        public double flowValue { get; set; }

        public string strategy { get; set; }

        //资源地址
        public string url { get; set; }

        public string creatorCode { get; set; }
        //创建时间
        public long createTime { get; set; }

        public string updatorCode { get; set; }
        //创建时间
        public long updateTime { get; set; }
        //源flowid
        public string originalFlowId { get; set; }
    }





    public class ReportModifyFlowInfo2
    {
        //流程编号
        public string id { get; set; }
        //租户编号
        public string tenantId { get; set; }
        //项目编号
        public string projectId { get; set; }
        //流程名称
        public string name { get; set; }
        //人工工时
        public double laborCost { get; set; }
        //流程价值
        public double flowValue { get; set; }

        public string strategy { get; set; }

        //资源地址
        public string url { get; set; }

        public string creatorCode { get; set; }
        //创建时间
        public long createTime { get; set; }

        public string updatorCode { get; set; }
        //创建时间
        public long updateTime { get; set; }

        public string originalFlowId { get; set; }
    }


}
