from PIL import Image


def display(image_data, width, height):

    image_list = []
    for i in range(width):
        for j in range(height):
            image_list.append(image_data[i][j])

    img = Image.new('RGB', (width, height))
    img.putdata(image_list)
    img.save('image.png')

