class LargeGroupServiceChargePolicy implements ServiceChargePolicy {
    @Override
    public double getChargeRate(int guestsCount) {
        return guestsCount >= 5 ? 0.10 : 0.0;
    }
}
