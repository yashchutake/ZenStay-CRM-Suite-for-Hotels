using System;
using System.Collections.Generic;

namespace ZenHotelManagement.WebApp.Utilities
{
    /// <summary>
    /// Centralized utility class for handling booking status badges across the application.
    /// Provides consistent styling, icons, and status mapping for both room and cab bookings.
    /// </summary>
    public static class BookingStatusBadge
    {
        #region Status Definitions

        /// <summary>
        /// Standard room booking status values used throughout the system
        /// </summary>
        public static class RoomBookingStatus
        {
            public const string Confirmed = "Confirmed";
            public const string CheckedIn = "Checked In";
            public const string CheckedOut = "Checked Out";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
            public const string NoShow = "No Show";
            public const string PartialPayment = "Partial Payment";
            public const string PaymentPending = "Payment Pending";
            public const string Refunded = "Refunded";
            public const string OnHold = "On Hold";
        }

        /// <summary>
        /// Cab booking status values
        /// </summary>
        public static class CabBookingStatus
        {
            public const string Confirmed = "Confirmed";
            public const string InProgress = "In Progress";
            public const string Completed = "Completed";
            public const string Cancelled = "Cancelled";
            public const string NoShow = "No Show";
        }

        /// <summary>
        /// Customer aggregate status values (for customer list)
        /// </summary>
        public static class CustomerStatus
        {
            public const string CheckedIn = "Checked In";
            public const string ActiveBooking = "Active Booking";
            public const string RecentGuest = "Recent Guest";
            public const string NoBookings = "No Bookings";
            public const string PendingPayment = "Pending Payment";
            public const string VIPGuest = "VIP Guest";
        }

        #endregion

        #region Badge Information Classes

        /// <summary>
        /// Contains all badge styling information
        /// </summary>
        public class BadgeInfo
        {
            public string CssClass { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string Color { get; set; } = string.Empty;
            public string TextColor { get; set; } = "white";
            public int Priority { get; set; } = 0; // Higher number = higher priority
            public string Description { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
        }

        #endregion

        #region Room Booking Badge Mappings

        /// <summary>
        /// Maps room booking statuses to their badge information
        /// </summary>
        private static readonly Dictionary<string, BadgeInfo> RoomBookingBadges = new(StringComparer.OrdinalIgnoreCase)
        {
            // Active/Positive States (Green Family)
            [RoomBookingStatus.Confirmed] = new BadgeInfo
            {
                CssClass = "bg-success",
                Icon = "bi-calendar-check-fill",
                Color = "#198754",
                Priority = 5,
                Description = "Booking confirmed and ready for check-in",
                Category = "Active"
            },

            // Current Occupancy (Blue Family)
            [RoomBookingStatus.CheckedIn] = new BadgeInfo
            {
                CssClass = "bg-primary",
                Icon = "bi-door-open-fill",
                Color = "#0d6efd",
                Priority = 10, // Highest priority for room bookings
                Description = "Customer currently occupying the room",
                Category = "Occupancy"
            },
            ["Checked-In"] = new BadgeInfo // Handle hyphenated version
            {
                CssClass = "bg-primary",
                Icon = "bi-door-open-fill",
                Color = "#0d6efd",
                Priority = 10,
                Description = "Customer currently occupying the room",
                Category = "Occupancy"
            },
            ["Cheked-In"] = new BadgeInfo // Handle typo version found in database
            {
                CssClass = "bg-primary",
                Icon = "bi-door-open-fill",
                Color = "#0d6efd",
                Priority = 10,
                Description = "Customer currently occupying the room",
                Category = "Occupancy"
            },

            // Completed States (Info/Cyan Family)
            [RoomBookingStatus.CheckedOut] = new BadgeInfo
            {
                CssClass = "bg-info",
                Icon = "bi-box-arrow-right-fill",
                Color = "#0dcaf0",
                Priority = 3,
                Description = "Customer has completed checkout",
                Category = "Completed"
            },
            ["Checked-Out"] = new BadgeInfo // Handle hyphenated version
            {
                CssClass = "bg-info",
                Icon = "bi-box-arrow-right-fill",
                Color = "#0dcaf0",
                Priority = 3,
                Description = "Customer has completed checkout",
                Category = "Completed"
            },
            [RoomBookingStatus.Completed] = new BadgeInfo
            {
                CssClass = "bg-info",
                Icon = "bi-check-all",
                Color = "#0dcaf0",
                Priority = 2,
                Description = "Booking cycle fully completed",
                Category = "Completed"
            },

            // Payment Related (Warning Family)
            [RoomBookingStatus.PartialPayment] = new BadgeInfo
            {
                CssClass = "bg-warning text-dark",
                Icon = "bi-currency-rupee",
                Color = "#ffc107",
                TextColor = "dark",
                Priority = 7,
                Description = "Partial payment received, balance pending",
                Category = "Payment"
            },
            [RoomBookingStatus.PaymentPending] = new BadgeInfo
            {
                CssClass = "bg-warning text-dark",
                Icon = "bi-exclamation-triangle-fill",
                Color = "#ffc107",
                TextColor = "dark",
                Priority = 8,
                Description = "Payment required before check-in/out",
                Category = "Payment"
            },

            // Negative States (Danger Family)
            [RoomBookingStatus.Cancelled] = new BadgeInfo
            {
                CssClass = "bg-danger",
                Icon = "bi-x-circle-fill",
                Color = "#dc3545",
                Priority = 1,
                Description = "Booking was cancelled",
                Category = "Cancelled"
            },
            [RoomBookingStatus.NoShow] = new BadgeInfo
            {
                CssClass = "bg-danger",
                Icon = "bi-person-x-fill",
                Color = "#dc3545",
                Priority = 1,
                Description = "Customer did not show up for booking",
                Category = "Cancelled"
            },

            // Special States
            [RoomBookingStatus.Refunded] = new BadgeInfo
            {
                CssClass = "bg-secondary",
                Icon = "bi-arrow-counterclockwise",
                Color = "#6c757d",
                Priority = 1,
                Description = "Payment has been refunded",
                Category = "Special"
            },
            [RoomBookingStatus.OnHold] = new BadgeInfo
            {
                CssClass = "bg-dark",
                Icon = "bi-pause-circle-fill",
                Color = "#212529",
                Priority = 6,
                Description = "Booking temporarily on hold",
                Category = "Special"
            }
        };

        #endregion

        #region Cab Booking Badge Mappings

        /// <summary>
        /// Maps cab booking statuses to their badge information
        /// </summary>
        private static readonly Dictionary<string, BadgeInfo> CabBookingBadges = new(StringComparer.OrdinalIgnoreCase)
        {
            [CabBookingStatus.Confirmed] = new BadgeInfo
            {
                CssClass = "bg-primary",
                Icon = "bi-calendar-check",
                Color = "#0d6efd",
                Priority = 5,
                Description = "Cab booking confirmed",
                Category = "Active"
            },
            [CabBookingStatus.InProgress] = new BadgeInfo
            {
                CssClass = "bg-warning text-dark",
                Icon = "bi-car-front-fill",
                Color = "#ffc107",
                TextColor = "dark",
                Priority = 10,
                Description = "Ride currently in progress",
                Category = "Active"
            },
            [CabBookingStatus.Completed] = new BadgeInfo
            {
                CssClass = "bg-success",
                Icon = "bi-check-circle-fill",
                Color = "#198754",
                Priority = 3,
                Description = "Ride completed successfully",
                Category = "Completed"
            },
            [CabBookingStatus.Cancelled] = new BadgeInfo
            {
                CssClass = "bg-danger",
                Icon = "bi-x-circle-fill",
                Color = "#dc3545",
                Priority = 1,
                Description = "Cab booking cancelled",
                Category = "Cancelled"
            },
            [CabBookingStatus.NoShow] = new BadgeInfo
            {
                CssClass = "bg-danger",
                Icon = "bi-person-x-fill",
                Color = "#dc3545",
                Priority = 1,
                Description = "Customer did not show up for pickup",
                Category = "Cancelled"
            }
        };

        #endregion

        #region Customer Status Badge Mappings

        /// <summary>
        /// Maps customer aggregate statuses to their badge information
        /// </summary>
        private static readonly Dictionary<string, BadgeInfo> CustomerStatusBadges = new(StringComparer.OrdinalIgnoreCase)
        {
            [CustomerStatus.CheckedIn] = new BadgeInfo
            {
                CssClass = "bg-success",
                Icon = "bi-check-circle-fill",
                Color = "#198754",
                Priority = 10, // Highest priority
                Description = "Customer currently checked in",
                Category = "Active"
            },
            [CustomerStatus.ActiveBooking] = new BadgeInfo
            {
                CssClass = "bg-primary",
                Icon = "bi-calendar-check",
                Color = "#0d6efd",
                Priority = 8,
                Description = "Has confirmed upcoming booking",
                Category = "Active"
            },
            [CustomerStatus.RecentGuest] = new BadgeInfo
            {
                CssClass = "bg-info",
                Icon = "bi-clock-history",
                Color = "#0dcaf0",
                Priority = 5,
                Description = "Recently completed stay",
                Category = "Completed"
            },
            [CustomerStatus.PendingPayment] = new BadgeInfo
            {
                CssClass = "bg-warning text-dark",
                Icon = "bi-exclamation-triangle-fill",
                Color = "#ffc107",
                TextColor = "dark",
                Priority = 9,
                Description = "Has pending payment due",
                Category = "Payment"
            },
            [CustomerStatus.VIPGuest] = new BadgeInfo
            {
                CssClass = "bg-gradient-gold text-dark",
                Icon = "bi-star-fill",
                Color = "#ffd700",
                TextColor = "dark",
                Priority = 7,
                Description = "VIP customer with premium status",
                Category = "Special"
            },
            [CustomerStatus.NoBookings] = new BadgeInfo
            {
                CssClass = "bg-light text-dark",
                Icon = "bi-info-circle",
                Color = "#f8f9fa",
                TextColor = "dark",
                Priority = 1,
                Description = "No booking history available",
                Category = "Neutral"
            }
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets badge information for a room booking status
        /// </summary>
        /// <param name="status">The booking status</param>
        /// <returns>Badge information or default badge if status not found</returns>
        public static BadgeInfo GetRoomBookingBadge(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return GetDefaultBadge("Unknown");

            return RoomBookingBadges.TryGetValue(status, out var badge) 
                ? badge 
                : GetDefaultBadge(status);
        }

        /// <summary>
        /// Gets badge information for a cab booking (using boolean completion status)
        /// </summary>
        /// <param name="isCompleted">Whether the ride is completed</param>
        /// <returns>Badge information</returns>
        public static BadgeInfo GetCabBookingBadge(bool? isCompleted)
        {
            var status = isCompleted == true ? CabBookingStatus.Completed : CabBookingStatus.Confirmed;
            return CabBookingBadges[status];
        }

        /// <summary>
        /// Gets badge information for a cab booking status
        /// </summary>
        /// <param name="status">The cab booking status</param>
        /// <returns>Badge information</returns>
        public static BadgeInfo GetCabBookingBadge(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return GetDefaultBadge("Unknown");

            return CabBookingBadges.TryGetValue(status, out var badge) 
                ? badge 
                : GetDefaultBadge(status);
        }

        /// <summary>
        /// Gets badge information for a customer status
        /// </summary>
        /// <param name="status">The customer status</param>
        /// <returns>Badge information</returns>
        public static BadgeInfo GetCustomerStatusBadge(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return GetDefaultBadge("Unknown");

            return CustomerStatusBadges.TryGetValue(status, out var badge) 
                ? badge 
                : GetDefaultBadge(status);
        }

        /// <summary>
        /// Generates HTML for a badge with icon
        /// </summary>
        /// <param name="status">The status text to display</param>
        /// <param name="badgeInfo">Badge styling information</param>
        /// <param name="size">Badge size (sm, md, lg)</param>
        /// <returns>HTML string for the badge</returns>
        public static string GenerateBadgeHtml(string status, BadgeInfo badgeInfo, string size = "md")
        {
            var sizeClass = size switch
            {
                "sm" => "badge-sm",
                "lg" => "badge-lg",
                _ => ""
            };

            return $@"<span class=""badge {badgeInfo.CssClass} {sizeClass}"" title=""{badgeInfo.Description}"">
                        <i class=""bi {badgeInfo.Icon} me-1""></i>
                        {status}
                      </span>";
        }

        /// <summary>
        /// Generates CSS for custom badge styling
        /// </summary>
        /// <returns>CSS string for badge customizations</returns>
        public static string GenerateBadgeStyles()
        {
            return @"
                <style>
                    .bg-gradient-gold {
                        background: linear-gradient(45deg, #ffd700, #ffed4e) !important;
                        color: #000 !important;
                    }
                    
                    .badge-sm {
                        font-size: 0.7rem;
                        padding: 0.25rem 0.4rem;
                    }
                    
                    .badge-lg {
                        font-size: 0.9rem;
                        padding: 0.5rem 0.75rem;
                    }
                    
                    .badge {
                        font-weight: 500;
                        border-radius: 6px;
                        display: inline-flex;
                        align-items: center;
                        gap: 0.25rem;
                    }
                    
                    .badge i {
                        font-size: 0.8em;
                    }
                    
                    .badge:hover {
                        transform: translateY(-1px);
                        box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                        transition: all 0.2s ease;
                    }
                </style>";
        }

        /// <summary>
        /// Gets default badge for unknown statuses
        /// </summary>
        /// <param name="status">The unknown status</param>
        /// <returns>Default badge information</returns>
        private static BadgeInfo GetDefaultBadge(string status)
        {
            return new BadgeInfo
            {
                CssClass = "bg-secondary",
                Icon = "bi-question-circle",
                Color = "#6c757d",
                Priority = 0,
                Description = $"Unknown status: {status}",
                Category = "Unknown"
            };
        }

        #endregion

        #region Status Calculation Helpers        /// <summary>
        /// Calculates the primary customer status based on their bookings
        /// </summary>
        /// <param name="roomBookings">Customer's room bookings</param>
        /// <param name="cabBookings">Customer's cab bookings</param>
        /// <returns>Primary customer status</returns>
        public static string CalculateCustomerStatus(IEnumerable<dynamic> roomBookings, IEnumerable<dynamic>? cabBookings = null)
        {
            var bookings = roomBookings?.ToList() ?? new List<dynamic>();
            
            if (!bookings.Any())
                return CustomerStatus.NoBookings;

            // Priority 1: Any checked-in booking
            var checkedInBooking = bookings.FirstOrDefault(b => 
                string.Equals(b.BookingStatus?.ToString(), RoomBookingStatus.CheckedIn, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(b.BookingStatus?.ToString(), "Checked-In", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(b.BookingStatus?.ToString(), "Cheked-In", StringComparison.OrdinalIgnoreCase));
            
            if (checkedInBooking != null)
                return CustomerStatus.CheckedIn;            // Priority 2: Active confirmed booking
            var activeBooking = bookings.FirstOrDefault(b => 
                string.Equals(b.BookingStatus?.ToString(), RoomBookingStatus.Confirmed, StringComparison.OrdinalIgnoreCase) &&
                b.CheckInDate != null && Convert.ToDateTime(b.CheckInDate) >= DateTime.Now);
            
            if (activeBooking != null)
                return CustomerStatus.ActiveBooking;

            // Priority 3: Recent completed stay (within last 30 days)
            var recentBooking = bookings.FirstOrDefault(b => 
                (string.Equals(b.BookingStatus?.ToString(), RoomBookingStatus.Completed, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(b.BookingStatus?.ToString(), RoomBookingStatus.CheckedOut, StringComparison.OrdinalIgnoreCase)) &&
                b.CheckOutDate != null && 
                Convert.ToDateTime(b.CheckOutDate) >= DateTime.Now.AddDays(-30));
            
            if (recentBooking != null)
                return CustomerStatus.RecentGuest;

            // Default: Show status of most recent booking
            var lastBooking = bookings
                .OrderByDescending(b => b.CheckInDate)
                .FirstOrDefault();
            
            return lastBooking?.BookingStatus?.ToString() ?? CustomerStatus.NoBookings;
        }

        #endregion
    }
}
