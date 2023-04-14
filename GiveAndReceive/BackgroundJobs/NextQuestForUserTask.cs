using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GiveAndReceive.BackgroundJobs
{
    public class NextQuestForUserTask : IJob
    {
      public async Task Execute(IJobExecutionContext context)
        {
            //lấy người đang làm nhiệm vụ hiện tại

            //lấy danh sách nhiệm vụ chưa làm

            #region nếu danh sách nhiệm vụ chưa làm vẫn còn

            // kiểm tra nhiệm vụ còn hạn không

            //nếu nhiệm vụ còn hạn, return

            //nếu nhiệm vụ hết hạn


            //thoát người dùng khỏi hàng đợi

            //chuyển tiếp nhiệm vụ cho người khác


            #endregion


            #region nếu danh sách nhiệm vụ chưa làm trống

            //chuyển trạng thái hàng đợi của người cho thành chờ xác nhận

            #endregion



        }
    }
}