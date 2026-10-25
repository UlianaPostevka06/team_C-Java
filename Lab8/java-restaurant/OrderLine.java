class OrderLine {
    private final MenuItem item;
    private final int count;

    public OrderLine(MenuItem item, int count) {
        if (item == null) throw new IllegalArgumentException("Страва обов'язкова");
        if (count <= 0) throw new IllegalArgumentException("Кількість має бути додатною");
        this.item = item;
        this.count = count;
    }

    // Information Expert: рядок володіє ціною та кількістю
    public double getLineTotal() {
        return item.getPrice() * count;
    }

    public MenuItem getItem() { return item; }
    public int getCount() { return count; }
}
