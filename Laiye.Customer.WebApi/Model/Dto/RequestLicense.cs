namespace Laiye.Customer.WebApi.Model.Dto
{
    public class RequestLicense
	{
		/// <summary>
		/// 异步标记：默认false；如果是true则必须提供callback接口，应用市场会定时调度。
		/// </summary>
		public string async { get; set; }
		///<added>
		///规格key：多规格使用“|”连接
		public string spec { get; set; }
		///<summary>
		///规格名字
		/// </summary>
		public string specNames { get; set; }
		///<summary>
		///规格值
		///</summary>
		///<summary>
		public string specValues { get; set; }
		///<summary>
		///计费方式 period 周期，buyOut 一次性
		/// </summary>
		public string chargeType { get; set; }

		///<summary>
		///价格
		/// </summary> 
		public string price { get; set; }

		// <summary>
		/// 订单编号系统生成的
		/// </summary>
		public string orderNo { get; set; }
		/// <summary>
		/// 产品编号
		/// </summary>
		public string productNo { get; set; }
		/// <summary>
		/// 版本号
		/// </summary>
		public string versionNo { get; set; }

		/// <summary>
		/// 渠道编码,表明请求来源appStore
		/// </summary>
		public string channel { get; set; }

		/// <summary>
		/// 租户编码
		/// </summary>
		public long tenant { get; set; }
		/// <summary>
		/// 项目id
		/// </summary>
		public string projectId { get; set; }
		/// <summary>
		/// 1-资源项目 2-开发项目
		/// </summary>
		public string projectType { get; set; }
		/// <summary>
		/// 域账号（加密）
		/// </summary>
		public string userName { get; set; }
		/// <summary>
		/// 用户类型：nuc，local（加密）
		/// </summary>
		public string userType { get; set; }
		/// <summary
		/// 请求时间
		/// </summary>
		public DateTime requestTime { get; set; }
		/// <summary>
		/// 到期日:yyyy-MM-dd,为空表示没有到期日。周期性商品，根据用户购买时所选周期获取此日期
		/// </summary>
		public DateTime dueDate { get; set; }
		/// <summary>
		/// license类型  1-试用 2-1年 3-买断
		/// </summary>
		public int licenseType { get; set; }
		/// <summary>
		/// license数量
		/// </summary>
		public int licenseCount { get; set; }
		/// <summary>
		/// 购买license类型1:command 2:worker 3:creator
		/// </summary>
		public int BuyLicenseType { get; set; }


		public RequestLicense()
		{

		}

		~RequestLicense()
		{

		}
	}
}
