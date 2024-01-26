const addToCart = (event, bookId) => {
    event.preventDefault();

    $.ajax({
        url: "/Customer/Home/AddToCart",
        type: "POST",
        data: {
            bookId: bookId,
        },
        success: function (data) {
            console.log(data);
            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: data.message,
                showConfirmButton: false,
                timer: 3000
            });


            //Cập nhât ở header là luôn luôn bởi vì là chung
            const totalBookElement = document.getElementById('total_book');
            if (totalBookElement) {
                totalBookElement.innerText = `(${data.totalProducts})`;
            }

            //Chỉ cập nhật ở trang Cart khi người dùng ở trang Cart

            //Cập nhật số lượng của sách hiện tại
            const totalQuantityElement = document.querySelector(`.total_quantity[data-book-id="${bookId}"]`);

            if (totalQuantityElement) {
                totalQuantityElement.innerText = `${data.itemsUpdated}`;
            }

            //Cập nhật tổng tiền
            const totalPriceElement = document.querySelector(".total_price");
            if (totalPriceElement) {

                const formattedPrice = new Intl.NumberFormat('en-US', {
                    style: 'currency',
                    currency: 'USD',
                    minimumFractionDigits: 2
                }).format(data.totalPrice);

                totalPriceElement.innerText = formattedPrice;
            }


        },
        error: function (data) {
            console.log(data);
            Swal.fire({
                icon: 'error',
                title: data.message,
                text: 'Bạn đã thêm vào giỏ hàng thất bại',
            });
        }
    });
}

const removeFromCart = (event, bookId, quantity) => {
    event.preventDefault();

    $.ajax({
        url: "/Customer/Cart/RemoveFromCart",
        type: "POST",
        data: {
            bookId: bookId,
            quantity: quantity,
        },
        success: function (data) {
            console.log(data);
            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: data.message,
                showConfirmButton: false,
                timer: 3000
            });


            //Cập nhât ở header là luôn luôn bởi vì là chung
            const totalBookElement = document.getElementById('total_book');
            if (totalBookElement) {
                totalBookElement.innerText = `(${data.totalProducts})`;
            }

            //Chỉ cập nhật ở trang Cart khi người dùng ở trang Cart

            //Cập nhật số lượng của sách hiện tại
            const totalQuantityElement = document.querySelector(`.total_quantity[data-book-id="${bookId}"]`);

            if (totalQuantityElement) {
                if (data.itemsUpdated == 0) {
                    const rowElement = event.target.closest('.parent-row');
                    if (rowElement) {
                        rowElement.style.display = 'none';
                    }
                }
                else {
                    totalQuantityElement.innerText = `${data.itemsUpdated}`;
                }
            }

            //Cập nhật tổng tiền
            const totalPriceElement = document.querySelector(".total_price");
            if (totalPriceElement) {

                const formattedPrice = new Intl.NumberFormat('en-US', {
                    style: 'currency',
                    currency: 'USD',
                    minimumFractionDigits: 2
                }).format(data.totalPrice);

                totalPriceElement.innerText = formattedPrice;
            }


        },
        error: function (data) {
            console.log(data);
            Swal.fire({
                icon: 'error',
                title: data.message,
                text: 'Bạn đã thêm vào giỏ hàng thất bại',
            });
        }
    });
}