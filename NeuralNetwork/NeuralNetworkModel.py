import pandas as pd
import numpy as np
import tensorflow as tf
import keras
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
import joblib

def main():
    print("Neural Network Model")

    # Load the dataset
    df = pd.read_csv('HistoricalNBAStats.csv')
    
    # Serate features and target variable
    X = df.drop(columns=['Output', 'Season', 'Team'])
    y = df['Output'] # Target between 0 to 5, where 0 is not making the playoffs and 5 is winning the championship

    #Standardize features
    scaler = StandardScaler()
    X_scale = scaler.fit_transform(X)

    #Save scaler for future
    joblib.dump(scaler, 'scaler.pkl')

    #Train/test split
    X_train, X_test, y_train, y_test = train_test_split(X_scale, y, test_size=0.2, random_state=42)

    # Build model
    model = tf.keras.Sequential([
        tf.keras.layers.Dense(64, activation='relu', input_shape=(X_train.shape[1],)),
        tf.keras.layers.Dense(32, activation='relu'),
        tf.keras.layers.Dense(6, activation='softmax')  # 6 Output neurons for 0-5 classification
    ])

    #Compile model
    model.compile(optimizer='adam', loss='sparse_categorical_crossentropy', metrics=['accuracy'])

    #Train
    model.fit(X_train, y_train, epochs=50, batch_size=8, validation_split=0.1)

    #Evaluate model
    loss, accuracy = model.evaluate(X_test, y_test)
    print(f"Test Accuracy: {accuracy:.2f}")

    #Save the model 
    model.save('nba_model.keras')


main()