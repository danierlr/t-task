using NotificationService.Application.Utils;
using NotificationService.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Pipeline {
    internal class ProviderLane {
        private readonly CapacityLimiter _capacityLimiter = new CapacityLimiter(100);

        private readonly int _numConcurrencySlots;

        private readonly INotificationProvider _provider;

        public ProviderLane(INotificationProvider provider, int initialNumConcurrencySlots) {
            _provider = provider;
            _numConcurrencySlots = initialNumConcurrencySlots;
        }


    }
}
