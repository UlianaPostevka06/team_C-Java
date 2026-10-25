class HappyHoursDiscountPolicy implements DiscountPolicy {
    @Override
    public double getDiscountRate(String timeSlot) {
        return "HappyHours".equalsIgnoreCase(timeSlot) ? 0.15 : 0.0;
    }
}
