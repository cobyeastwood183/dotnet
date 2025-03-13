using Sentry;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace App.Services
{
    public static class CronJobService
    {
        /// <summary>
        /// Executes a scheduled job with Sentry cron monitoring
        /// </summary>
        /// <returns>IResult containing the job execution status</returns>
        public static IResult ExecuteScheduledJob()
        {
            string monitorSlug = "test-cron";

            var checkInId = SentrySdk.CaptureCheckIn(monitorSlug, CheckInStatus.InProgress);

            Console.WriteLine($"  Monitor Slug: {monitorSlug}");
            Console.WriteLine($"  Check-in ID: {checkInId}");

            try
            {
                Thread.Sleep(4000); // 4-second delay

                SentrySdk.CaptureCheckIn(monitorSlug, CheckInStatus.Ok, checkInId);
                
                return Results.Ok(new { Status = "Success", Message = "Scheduled job completed", Timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureCheckIn(monitorSlug, CheckInStatus.Error, checkInId);
                SentrySdk.CaptureException(ex);
                
                return Results.Problem("Scheduled job failed", statusCode: 500);
            }
        }

        /// <summary>
        /// Configures the cron job endpoints in the application
        /// </summary>
        /// <param name="app">The WebApplication to configure</param>
        public static void ConfigureCronJobEndpoints(WebApplication app)
        {
            app.MapGet("/api/scheduled-job", ExecuteScheduledJob);
        }
    }
} 